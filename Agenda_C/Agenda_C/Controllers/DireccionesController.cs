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
    [Authorize(Roles = "Administrador,Paciente,Profesional")]
    public class DireccionesController : Controller
    {
        private readonly AgendaContext _context;
        private readonly UserManager<Persona> _userManager;

        public DireccionesController(AgendaContext context, UserManager<Persona> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Direccion
        public async Task<IActionResult> Index()
        {
            // Incluimos la persona para mostrar a quién pertenece cada dirección
            var direcciones = await _context.Direcciones
                .Include(d => d.Persona)
                .OrderBy(d => d.Calle)
                .ThenBy(d => d.Altura)
                .ToListAsync();

            return View(direcciones);
        }

        // GET: Direccion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var direccion = await _context.Direcciones
                .Include(d => d.Persona)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (direccion == null)
            {
                return NotFound();
            }

            return View(direccion);
        }

        // GET: Direccion/Create
        public IActionResult Create()
        {
            // Mejoramos el dropdown para que muestre nombre completo en lugar de solo apellido
            ViewData["Personas"] = new SelectList(
                _context.Persona.OrderBy(p => p.Apellido).ThenBy(p => p.Nombre),
                "Id",
                "NombreCompleto"
            );

            return View();
        }

        // POST: Direccion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Calle,Altura,Piso,Departamento,PersonaId")] Direccion direccion)
        {
            // VALIDACIÓN: Altura debe ser mayor a 0
            if (direccion.Altura <= 0)
            {
                ModelState.AddModelError("Altura", 
                    "La altura debe ser un número positivo mayor a 0.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(direccion);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "✅ Dirección creada exitosamente.";
                TempData["TipoMensaje"] = "success";

                return RedirectToAction(nameof(Index));
            }

            ViewData["Personas"] = new SelectList(
                _context.Persona.OrderBy(p => p.Apellido).ThenBy(p => p.Nombre),
                "Id",
                "NombreCompleto",
                direccion.PersonaId
            );

            return View(direccion);
        }

        // GET: Direccion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var direccion = await _context.Direcciones.FindAsync(id);
            if (direccion == null)
            {
                return NotFound();
            }

            ViewData["Personas"] = new SelectList(
                _context.Persona.OrderBy(p => p.Apellido).ThenBy(p => p.Nombre),
                "Id",
                "NombreCompleto",
                direccion.PersonaId
            );

            return View(direccion);
        }

        // POST: Direccion/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Calle,Altura,Piso,Departamento,PersonaId")] Direccion direccion)
        {
            if (id != direccion.Id)
            {
                return NotFound();
            }

            // VALIDACIÓN DE SEGURIDAD: Verificar que la dirección pertenezca al usuario actual
            var currentUser = await _userManager.GetUserAsync(User);
            var direccionOriginal = await _context.Direcciones
                .Include(d => d.Persona)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);

            if (direccionOriginal != null && direccionOriginal.PersonaId != currentUser.Id && !User.IsInRole("Administrador"))
            {
                return Forbid();
            }

            // VALIDACIÓN: Altura debe ser mayor a 0
            if (direccion.Altura <= 0)
            {
                ModelState.AddModelError("Altura", 
                    "La altura debe ser un número positivo mayor a 0.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(direccion);
                    await _context.SaveChangesAsync();

                    TempData["Mensaje"] = "✅ Dirección actualizada correctamente.";
                    TempData["TipoMensaje"] = "success";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DireccionExists(direccion.Id))
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

            ViewData["Personas"] = new SelectList(
                _context.Persona.OrderBy(p => p.Apellido).ThenBy(p => p.Nombre),
                "Id",
                "NombreCompleto",
                direccion.PersonaId
            );

            return View(direccion);
        }

        // GET: Direccion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var direccion = await _context.Direcciones
                .Include(d => d.Persona)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (direccion == null)
            {
                return NotFound();
            }

            // Usamos el helper para verificar si está asociada a una persona
            ViewBag.TienePersona = await ValidationHelper.DireccionTienePersonaAsync(_context, id.Value);

            return View(direccion);
        }

        // POST: Direccion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var direccion = await _context.Direcciones
                .Include(d => d.Persona)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (direccion == null)
            {
                return NotFound();
            }

            // VALIDACIÓN: Usamos el helper para verificar si está asociada a una persona
            if (await ValidationHelper.DireccionTienePersonaAsync(_context, id))
            {
                TempData["Mensaje"] = "❌ No se puede eliminar esta dirección porque está asociada a una persona. " +
                                     "Primero hay que desvincularla o eliminar la persona.";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Direcciones.Remove(direccion);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "✅ Dirección eliminada exitosamente.";
                TempData["TipoMensaje"] = "success";
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"❌ Error al eliminar la dirección: {ex.Message}";
                TempData["TipoMensaje"] = "danger";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool DireccionExists(int id)
        {
            return _context.Direcciones.Any(e => e.Id == id);
        }
    }
}
