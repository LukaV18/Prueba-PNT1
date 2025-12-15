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
    [Authorize(Roles = "Administrador,Paciente,Profesional")]
    public class PacientesController : Controller
    {
        private readonly AgendaContext _context;
        private readonly UserManager<Persona> _userManager;

        public PacientesController(AgendaContext context, UserManager<Persona> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Pacientes
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Administrador") || User.IsInRole("Profesional"))
            {
                // Admin y profesionales ven todos los pacientes
                var pacientes = await _context.Pacientes
                    .Include(p => p.Cobertura)
                    .OrderBy(p => p.Apellido)
                    .ThenBy(p => p.Nombre)
                    .ToListAsync();

                return View(pacientes);
            }
            else if (User.IsInRole("Paciente"))
            {
                // Paciente solo ve su propio perfil
                var currentUser = await _userManager.GetUserAsync(User);
                return RedirectToAction("Details", new { id = currentUser.Id });
            }

            return Forbid();
        }

        // GET: Pacientes/Details/ID
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paciente = await _context.Pacientes
                .Include(p => p.Cobertura)
                .Include(p => p.Direccion)
                .Include(p => p.Telefonos)
                .Include(p => p.Turnos)
                    .ThenInclude(t => t.Profesional)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (paciente == null)
            {
                return NotFound();
            }

            return View(paciente);
        }

        // GET: Pacientes/Create
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            ViewData["Coberturas"] = new SelectList(
                _context.Coberturas
                    .Include(c => c.Paciente)
                    .OrderBy(c => c.Prestadora)
                    .Select(c => new { c.Id, Display = $"{c.Prestadora} - {c.NumeroCredencial}" }),
                "Id",
                "Display"
            );

            return View();
        }

        // POST: Pacientes/Create
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserName,Email,Nombre,Apellido,DNI,FechaDeNacimiento")] Paciente paciente)
        {
            if (await ValidationHelper.DniExisteAsync(_context, paciente.DNI))
            {
                ModelState.AddModelError("DNI", $"Ya existe un paciente con DNI {paciente.DNI}.");
            }

            if (await ValidationHelper.EmailExisteAsync(_context, paciente.Email))
            {
                ModelState.AddModelError("Email", $"El email {paciente.Email} ya está registrado.");
            }

            if (await ValidationHelper.UserNameExisteAsync(_context, paciente.UserName))
            {
                ModelState.AddModelError("UserName", $"El nombre de usuario {paciente.UserName} ya está en uso.");
            }

            if (ModelState.IsValid)
            {
                // Crear paciente con UserManager
                var result = await _userManager.CreateAsync(paciente, "Password1!");

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(paciente, "Paciente");
                    TempData["Mensaje"] = "✅ Paciente creado exitosamente.";
                    TempData["TipoMensaje"] = "success";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewData["Coberturas"] = new SelectList(
                _context.Coberturas
                    .Include(c => c.Paciente)
                    .OrderBy(c => c.Prestadora)
                    .Select(c => new { c.Id, Display = $"{c.Prestadora} - {c.NumeroCredencial}" }),
                "Id",
                "Display"
            );

            return View(paciente);
        }

        // GET: Pacientes/Edit/ID
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paciente = await _context.Pacientes.FindAsync(id);
            if (paciente == null)
            {
                return NotFound();
            }

            ViewData["Coberturas"] = new SelectList(
                _context.Coberturas
                    .Include(c => c.Paciente)
                    .OrderBy(c => c.Prestadora)
                    .Select(c => new { c.Id, Display = $"{c.Prestadora} - {c.NumeroCredencial}" }),
                "Id",
                "Display"
            );

            return View(paciente);
        }

        // POST: Pacientes/Edit/ID
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,FechaDeNacimiento,CoberturaId")] Paciente paciente)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            // Verificar permisos
            if (id != currentUser.Id && !User.IsInRole("Administrador"))
            {
                return Forbid();
            }

            if (id != paciente.Id)
            {
                return NotFound();
            }

            var pacienteOriginal = await _context.Pacientes
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pacienteOriginal == null)
            {
                return NotFound();
            }

            // NOTA: Eliminamos validaciones de Email y UserName porque ya no se pueden editar

            if (ModelState.IsValid)
            {
                try
                {
                    // Restauramos los datos inmutables
                    paciente.DNI = pacienteOriginal.DNI;
                    paciente.Nombre = pacienteOriginal.Nombre;
                    paciente.Apellido = pacienteOriginal.Apellido;
                    paciente.Email = pacienteOriginal.Email;
                    paciente.UserName = pacienteOriginal.UserName;
                    // Normalizamos el NormalizedUserName/Email por las dudas, aunque Identity lo maneja
                    paciente.NormalizedUserName = pacienteOriginal.NormalizedUserName;
                    paciente.NormalizedEmail = pacienteOriginal.NormalizedEmail;

                    _context.Update(paciente);
                    await _context.SaveChangesAsync();

                    TempData["Mensaje"] = "✅ Paciente actualizado correctamente.";
                    TempData["TipoMensaje"] = "success";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PacienteExists(paciente.Id))
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

            ViewData["Coberturas"] = new SelectList(
                _context.Coberturas
                    .Include(c => c.Paciente)
                    .OrderBy(c => c.Prestadora)
                    .Select(c => new { c.Id, Display = $"{c.Prestadora} - {c.NumeroCredencial}" }),
                "Id",
                "Display"
            );

            return View(paciente);
        }

        private bool PacienteExists(string id)
        {
            return _context.Pacientes.Any(e => e.Id == id);
        }
    }

}





