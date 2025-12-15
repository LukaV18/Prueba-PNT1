using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Agenda_C.Data;
using Agenda_C.Models;
using Agenda_C.Helpers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Agenda_C.Controllers
{
    // NOTA: Renombramos todo a plural porque el profe marcó que solo PacientesController
    // estaba bien y el resto estaba en singular. Ahora todos en plural para ser consistentes.
    public class ProfesionalesController : Controller
    {
        private readonly AgendaContext _context;
        private readonly UserManager<Persona> _userManager;

        public ProfesionalesController(AgendaContext context, UserManager<Persona> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Profesionales
        // Acceso anónimo permitido para listar profesionales
        public async Task<IActionResult> Index()
        {
            var profesionales = await _context.Profesionales
                .Include(p => p.Matricula)
                .Include(p => p.Prestacion)
                .OrderBy(p => p.Apellido)
                .ThenBy(p => p.Nombre)
                .ToListAsync();

            return View(profesionales);
        }

        // GET: Profesional/Details/5
        public async Task<IActionResult> Details(String id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Traemos todo lo que necesitamos para mostrar el detalle completo
            var profesional = await _context.Profesionales
                .Include(p => p.Matricula)
                .Include(p => p.Prestacion)
                .Include(p => p.Direccion)
                .Include(p => p.Telefonos)
                .Include(p => p.Turnos)
                    .ThenInclude(t => t.Paciente)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (profesional == null)
            {
                return NotFound();
            }

            // CAMBIO EP4: Cálculo de "Valor a percibir" (Checklist: "Visualizará valor a percibir ese mes")
            // Sumamos el precio de la prestación de todos los turnos confirmados del MES PASADO
            var fechaMesPasado = DateTime.Now.AddMonths(-1);
            var mesPasado = fechaMesPasado.Month;
            var anioPasado = fechaMesPasado.Year;
            
            decimal valorAPercibir = 0;
            if (profesional.Turnos != null && profesional.Prestacion != null)
            {
                valorAPercibir = profesional.Turnos
                    .Where(t => t.Confirmado && t.FechaHora.Month == mesPasado && t.FechaHora.Year == anioPasado)
                    .Count() * profesional.Prestacion.Precio;
            }
            
            ViewBag.ValorAPercibir = valorAPercibir;
            ViewBag.MesActual = fechaMesPasado.ToString("MMMM yyyy");

            return View(profesional);
        }

        // GET: Profesional/Create
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            // Mejoramos los dropdowns para que muestren info útil
            ViewData["Matriculas"] = new SelectList(
                _context.Matriculas
                    .OrderBy(m => m.Tipo)
                    .ThenBy(m => m.Numero)
                    .Select(m => new { 
                        m.Id, 
                        Display = $"{m.Tipo} - N° {m.Numero} - {m.Provincia}" 
                    }),
                "Id",
                "Display"
            );

            ViewData["Prestaciones"] = new SelectList(
                _context.Prestaciones
                    .OrderBy(p => p.Nombre),
                "Id",
                "Nombre"
            );

            return View();
        }

        // POST: Profesional/Create
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MatriculaId,PrestacionId,HoraInicio,HoraFin,UserName,Email,Nombre,Apellido,DNI,FechaDeNacimiento")] Profesional profesional)
        {
            // VALIDACIÓN 1: HoraInicio debe ser menor a HoraFin
            if (profesional.HoraInicio >= profesional.HoraFin)
            {
                ModelState.AddModelError("HoraFin", 
                    "La hora de fin debe ser posterior a la hora de inicio. Revisá bien los horarios.");
            }

            // VALIDACIÓN 2: DNI único usando el helper
            if (await ValidationHelper.DniExisteAsync(_context, profesional.DNI))
            {
                ModelState.AddModelError("DNI", 
                    $"Ya existe un profesional con DNI {profesional.DNI}.");
            }

            // VALIDACIÓN 3: Email único usando el helper
            if (await ValidationHelper.EmailExisteAsync(_context, profesional.Email))
            {
                ModelState.AddModelError("Email", 
                    $"El email {profesional.Email} ya está registrado.");
            }

            // VALIDACIÓN 4: UserName único usando el helper
            if (await ValidationHelper.UserNameExisteAsync(_context, profesional.UserName))
            {
                ModelState.AddModelError("UserName", 
                    $"El nombre de usuario {profesional.UserName} ya está en uso.");
            }

            if (ModelState.IsValid)
            {
                // Crear con UserManager
                var result = await _userManager.CreateAsync(profesional, "Password1!");

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(profesional, "Profesional");

                    TempData["Mensaje"] = $"Profesional creado. Usuario: {profesional.Email}, Contraseña: Password1!";
                    TempData["TipoMensaje"] = "success";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Si hay errores, volvemos a cargar los dropdowns
            ViewData["Matriculas"] = new SelectList(
                _context.Matriculas
                    .OrderBy(m => m.Tipo)
                    .ThenBy(m => m.Numero)
                    .Select(m => new { 
                        m.Id, 
                        Display = $"{m.Tipo} - N° {m.Numero} - {m.Provincia}" 
                    }),
                "Id",
                "Display",
                profesional.MatriculaId
            );

            ViewData["Prestaciones"] = new SelectList(
                _context.Prestaciones.OrderBy(p => p.Nombre),
                "Id",
                "Nombre",
                profesional.PrestacionId
            );

            return View(profesional);
        }

        // GET: Profesional/Edit/5
        [Authorize(Roles = "Administrador,Profesional")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profesional = await _context.Profesionales.FindAsync(id);
            if (profesional == null)
            {
                return NotFound();
            }

            // VALIDACIÓN DE SEGURIDAD: Verificar que el usuario sea Admin o el mismo profesional
            var currentUser = await _userManager.GetUserAsync(User);
            if (!User.IsInRole("Administrador") && currentUser.Id != id)
            {
                return Forbid();
            }

            ViewData["Matriculas"] = new SelectList(
                _context.Matriculas
                    .OrderBy(m => m.Tipo)
                    .ThenBy(m => m.Numero)
                    .Select(m => new { 
                        m.Id, 
                        Display = $"{m.Tipo} - N° {m.Numero} - {m.Provincia}" 
                    }),
                "Id",
                "Display",
                profesional.MatriculaId
            );

            ViewData["Prestaciones"] = new SelectList(
                _context.Prestaciones.OrderBy(p => p.Nombre),
                "Id",
                "Nombre",
                profesional.PrestacionId
            );

            return View(profesional);
        }

        // POST: Profesional/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Profesional")]
        public async Task<IActionResult> Edit(string id, [Bind("Id,MatriculaId,PrestacionId,HoraInicio,HoraFin,UserName,Email,FechaDeNacimiento")] Profesional profesional)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            // Verificar permisos
            if (id != currentUser.Id && !User.IsInRole("Administrador"))
            {
                return Forbid();
            }

            // NOTA: igual que Pacientes, DNI/Nombre/Apellido no están en Bind porque no se pueden modificar
            
            if (id != profesional.Id)
            {
                return NotFound();
            }

            // Traemos el profesional original para mantener sus datos inmutables
            var profesionalOriginal = await _context.Profesionales
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (profesionalOriginal == null)
            {
                return NotFound();
            }

            // VALIDACIÓN 1: HoraInicio debe ser menor a HoraFin
            if (profesional.HoraInicio >= profesional.HoraFin)
            {
                ModelState.AddModelError("HoraFin", 
                    "La hora de fin debe ser posterior a la hora de inicio.");
            }

            // VALIDACIÓN 2: Email único usando el helper (excluyendo el actual)
            if (await ValidationHelper.EmailExisteAsync(_context, profesional.Email, id))
            {
                ModelState.AddModelError("Email", 
                    $"El email {profesional.Email} ya está registrado por otro profesional.");
            }

            // VALIDACIÓN 3: UserName único usando el helper (excluyendo el actual)
            if (await ValidationHelper.UserNameExisteAsync(_context, profesional.UserName, id))
            {
                ModelState.AddModelError("UserName", 
                    $"El nombre de usuario {profesional.UserName} ya está en uso.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Restauramos los campos inmutables del profesional original
                    profesional.DNI = profesionalOriginal.DNI;
                    profesional.Nombre = profesionalOriginal.Nombre;
                    profesional.Apellido = profesionalOriginal.Apellido;

                    _context.Update(profesional);
                    await _context.SaveChangesAsync();

                    TempData["Mensaje"] = "✅ Profesional actualizado correctamente.";
                    TempData["TipoMensaje"] = "success";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProfesionalExists(profesional.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["Matriculas"] = new SelectList(
                _context.Matriculas
                    .OrderBy(m => m.Tipo)
                    .ThenBy(m => m.Numero)
                    .Select(m => new { 
                        m.Id, 
                        Display = $"{m.Tipo} - N° {m.Numero} - {m.Provincia}" 
                    }),
                "Id",
                "Display",
                profesional.MatriculaId
            );

            ViewData["Prestaciones"] = new SelectList(
                _context.Prestaciones.OrderBy(p => p.Nombre),
                "Id",
                "Nombre",
                profesional.PrestacionId
            );

            return View(profesional);
        }

        // GET: Profesionales/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profesional = await _context.Profesionales
                .Include(p => p.Matricula)
                .Include(p => p.Prestacion)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (profesional == null)
            {
                return NotFound();
            }

            // Usamos el helper para verificar si tiene turnos asignados
            var tieneTurnos = await ValidationHelper.ProfesionalTieneTurnosAsync(_context, id);
            ViewBag.TieneTurnos = tieneTurnos;

            return View(profesional);
        }

        // POST: Profesionales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var profesional = await _context.Profesionales.FindAsync(id);

            if (profesional == null)
            {
                return NotFound();
            }

            // VALIDACIÓN CRÍTICA: usamos el helper para verificar turnos
            var tieneTurnos = await ValidationHelper.ProfesionalTieneTurnosAsync(_context, id);
            
            if (tieneTurnos)
            {
                TempData["Mensaje"] = "❌ No se puede eliminar este profesional porque tiene turnos asignados. Primero hay que cancelar o reasignar los turnos.";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Profesionales.Remove(profesional);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "✅ Profesional eliminado exitosamente.";
                TempData["TipoMensaje"] = "success";
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"❌ Error al eliminar el profesional: {ex.Message}";
                TempData["TipoMensaje"] = "danger";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProfesionalExists(String id)
        {
            return _context.Profesionales.Any(e => e.Id == id);
        }
    }
}
