using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Agenda_C.Data;
using Agenda_C.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Agenda_C.Controllers
{
    // NOTA: Renombramos todo a plural porque el profe marcó que solo PacientesController
    // estaba bien y el resto estaba en singular. Ahora todos en plural para ser consistentes.
    [Authorize]
    public class FormulariosController : Controller
    {
        private readonly AgendaContext _context;
        private readonly UserManager<Persona> _userManager;

        public FormulariosController(AgendaContext context, UserManager<Persona> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Formulario
        // NOTA ALEX: agregué filtros para ver Leídos/No leídos/Todos
        // CAMBIO EP4: Solo Admin puede ver el listado de formularios
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Index(string filtro = "todos")
        {
            var formularios = _context.Formularios
                .Include(f => f.Cobertura)
                .Include(f => f.Paciente)
                .OrderByDescending(f => f.Fecha); // Los más recientes primero

            IQueryable<Formulario> query = filtro?.ToLower() switch
            {
                "leidos" => formularios.Where(f => f.Leido),
                "noleidos" => formularios.Where(f => !f.Leido),
                _ => formularios // "todos" o cualquier otro valor
            };

            ViewBag.FiltroActual = filtro;
            return View(await query.ToListAsync());
        }

        // GET: Formulario/Details/5
        // CAMBIO EP4: Solo Admin puede ver detalles
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var formulario = await _context.Formularios
                .Include(f => f.Cobertura)
                .Include(f => f.Paciente)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (formulario == null)
            {
                return NotFound();
            }

            return View(formulario);
        }

        // GET: Formulario/Create
        // CAMBIO EP4: Pacientes pueden cargar formularios (Checklist: "Carga de formulario")
        // Permitimos acceso anónimo para que cualquiera pueda dejar consulta
        [AllowAnonymous]
        public async Task<IActionResult> Create()
        {
            // Cargamos las coberturas con un texto legible (Prestadora - NumeroCredencial)
            var coberturas = await _context.Coberturas.ToListAsync();
            ViewData["CoberturaId"] = new SelectList(
                coberturas.Select(c => new { c.Id, Descripcion = $"{c.Prestadora} - {c.NumeroCredencial}" }),
                "Id", 
                "Descripcion"
            );

            var modelo = new Formulario();

            if (User.Identity.IsAuthenticated)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    modelo.Nombre = currentUser.Nombre;
                    modelo.Apellido = currentUser.Apellido;
                    modelo.Email = currentUser.Email;
                    modelo.DNI = currentUser.DNI;
                    // Si el usuario tiene cobertura, podríamos pre-seleccionarla
                }
            }

            return View(modelo);
        }

        // POST: Formulario/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Create([Bind("Id,Motivo,CoberturaId,Mensaje,Titulo,Nombre,Apellido,Email,DNI,Prestadora")] Formulario formulario)
        {
            // Asignar fecha actual
            formulario.Fecha = DateTime.Now;
            formulario.Leido = false;

            // Asignar paciente actual si existe
            if (User.Identity.IsAuthenticated)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    formulario.PacienteId = currentUser.Id;
                    // Forzamos los datos del usuario logueado para evitar spoofing
                    formulario.Nombre = currentUser.Nombre;
                    formulario.Apellido = currentUser.Apellido;
                    formulario.Email = currentUser.Email;
                    formulario.DNI = currentUser.DNI;
                }
            }
            
            // Validaciones manuales si es anónimo ya las hace el modelo (Required)
            
            if (ModelState.IsValid)
            {
                _context.Add(formulario);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "✅ Formulario enviado exitosamente.";
                TempData["TipoMensaje"] = "success";
                return RedirectToAction("Index", "Home");
            }
            
            // Recargamos las coberturas con un texto legible
            var coberturas = await _context.Coberturas.ToListAsync();
            ViewData["CoberturaId"] = new SelectList(
                coberturas.Select(c => new { c.Id, Descripcion = $"{c.Prestadora} - {c.NumeroCredencial}" }),
                "Id", 
                "Descripcion",
                formulario.CoberturaId
            );
            return View(formulario);
        }

        // GET: Formulario/Edit/5
        // CAMBIO EP4: Solo Admin puede editar (para marcar como leído)
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var formulario = await _context.Formularios.FindAsync(id);
            if (formulario == null)
            {
                return NotFound();
            }

            // SOLO mostramos el campo Leido para editar, nada más
            return View(formulario);
        }

        // POST: Formulario/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Leido")] Formulario formulario)
        {
            // IMPORTANTE: SOLO permitimos editar el campo Leido
            // Esto es para que el admin pueda marcar formularios como leídos/no leídos
            // pero sin poder modificar el contenido original del formulario
            
            if (id != formulario.Id)
            {
                return NotFound();
            }

            // Traemos el formulario original completo
            var formularioOriginal = await _context.Formularios
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == id);

            if (formularioOriginal == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Restauramos TODOS los campos menos Leido
                    formulario.Email = formularioOriginal.Email;
                    formulario.Fecha = formularioOriginal.Fecha;
                    formulario.Nombre = formularioOriginal.Nombre;
                    formulario.Apellido = formularioOriginal.Apellido;
                    formulario.DNI = formularioOriginal.DNI;
                    formulario.Titulo = formularioOriginal.Titulo;
                    formulario.Mensaje = formularioOriginal.Mensaje;
                    formulario.CoberturaId = formularioOriginal.CoberturaId;
                    formulario.Prestadora = formularioOriginal.Prestadora;
                    formulario.PacienteId = formularioOriginal.PacienteId;
                    // Solo mantenemos el Leido que viene del form

                    _context.Update(formulario);
                    await _context.SaveChangesAsync();

                    string mensaje = formulario.Leido 
                        ? "✅ Formulario marcado como leído." 
                        : "✅ Formulario marcado como no leído.";
                    
                    TempData["Mensaje"] = mensaje;
                    TempData["TipoMensaje"] = "success";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FormularioExists(formulario.Id))
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

            return View(formulario);
        }

        private bool FormularioExists(int id)
        {
            return _context.Formularios.Any(e => e.Id == id);
        }
    }
}
