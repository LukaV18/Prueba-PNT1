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
    public class TurnosController : Controller
    {
        private readonly AgendaContext _context;
        private readonly UserManager<Persona> _userManager;

        public TurnosController(AgendaContext context, UserManager<Persona> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Turno
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            IQueryable<Turno> turnosQuery = _context.Turnos
                .Include(t => t.Paciente)
                .Include(t => t.Profesional);

            if (User.IsInRole("Administrador"))
            {
                // Admin ve todo
            }
            else if (User.IsInRole("Profesional"))
            {
                // Profesional ve sus turnos
                turnosQuery = turnosQuery.Where(t => t.ProfesionalId == currentUser.Id);
            }
            else if (User.IsInRole("Paciente"))
            {
                // Paciente ve sus turnos ("Mis Turnos")
                turnosQuery = turnosQuery.Where(t => t.PacienteId == currentUser.Id);
            }
            else
            {
                // Si no tiene rol conocido (ej. recién registrado sin rol), no ve nada o ve vacío
                turnosQuery = turnosQuery.Where(t => false);
            }

            return View(await turnosQuery.OrderByDescending(t => t.FechaHora).ToListAsync());
        }

        // GET: Turno/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var turno = await _context.Turnos
                .Include(t => t.Paciente)
                .Include(t => t.Profesional)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (turno == null)
            {
                return NotFound();
            }

            // Validar permisos: Paciente solo puede ver sus propios turnos
            var currentUser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Paciente") && turno.PacienteId != currentUser.Id)
            {
                return Forbid();
            }
            // Profesional solo puede ver sus propios turnos
            if (User.IsInRole("Profesional") && turno.ProfesionalId != currentUser.Id)
            {
                return Forbid();
            }

            return View(turno);
        }

        // GET: Turno/Create
        public async Task<IActionResult> Create()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            
            if (User.IsInRole("Paciente"))
            {
                // Pre-carga de datos del paciente logueado
                ViewData["PacienteId"] = new SelectList(_context.Pacientes, "Id", "NombreCompleto", currentUser.Id);
                ViewBag.IsPaciente = true; // Para deshabilitar el dropdown en la vista si se quiere
            }
            else
            {
                ViewData["PacienteId"] = new SelectList(_context.Pacientes, "Id", "NombreCompleto");
                ViewBag.IsPaciente = false;
            }

            ViewData["ProfesionalId"] = new SelectList(_context.Profesionales, "Id", "NombreCompleto");
            return View();
        }

        // POST: Turno/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FechaHora,PacienteId,ProfesionalId")] Turno turno)
        {
            // Valores automáticos que no vienen del form
            turno.FechaDeAlta = DateTime.Now;
            turno.Confirmado = false;  // Por defecto sin confirmar hasta que un admin o profesional lo confirme
            turno.Activo = true;       // Se crea activo

            // Validar que el paciente no tenga ya un turno activo
            var pacienteTieneTurnoActivo = await _context.Turnos
                .AnyAsync(t => t.PacienteId == turno.PacienteId && t.Activo == true && t.Id != turno.Id);
            
            if (pacienteTieneTurnoActivo)
            {
                ModelState.AddModelError("", "El paciente ya tiene un turno activo. No puede solicitar otro hasta que se atienda o cancele el actual.");
                ViewData["PacienteId"] = new SelectList(_context.Pacientes, "Id", "NombreCompleto", turno.PacienteId);
                ViewData["ProfesionalId"] = new SelectList(_context.Profesionales, "Id", "NombreCompleto", turno.ProfesionalId);
                return View(turno);
            }

            // Validar que solo sea lunes a viernes
            if (turno.FechaHora.DayOfWeek == DayOfWeek.Saturday || turno.FechaHora.DayOfWeek == DayOfWeek.Sunday)
            {
                ModelState.AddModelError("FechaHora", "Solo se pueden asignar turnos en días hábiles (lunes a viernes).");
                ViewData["PacienteId"] = new SelectList(_context.Pacientes, "Id", "NombreCompleto", turno.PacienteId);
                ViewData["ProfesionalId"] = new SelectList(_context.Profesionales, "Id", "NombreCompleto", turno.ProfesionalId);
                return View(turno);
            }

            // Validar rango de fechas: solo desde hoy hasta 7 días
            var hoy = DateTime.Now.Date;
            var limite = hoy.AddDays(7);
            if (turno.FechaHora.Date < hoy || turno.FechaHora.Date > limite)
            {
                ModelState.AddModelError("FechaHora", "Solo se pueden solicitar turnos con hasta 7 días de anticipación.");
                ViewData["PacienteId"] = new SelectList(_context.Pacientes, "Id", "NombreCompleto", turno.PacienteId);
                ViewData["ProfesionalId"] = new SelectList(_context.Profesionales, "Id", "NombreCompleto", turno.ProfesionalId);
                return View(turno);
            }

            // Obtener el profesional para validar horarios
            var profesional = await _context.Profesionales
                .Include(p => p.Prestacion)
                .FirstOrDefaultAsync(p => p.Id == turno.ProfesionalId);

            if (profesional == null)
            {
                ModelState.AddModelError("ProfesionalId", "Profesional no encontrado.");
                ViewData["PacienteId"] = new SelectList(_context.Pacientes, "Id", "NombreCompleto", turno.PacienteId);
                ViewData["ProfesionalId"] = new SelectList(_context.Profesionales, "Id", "NombreCompleto", turno.ProfesionalId);
                return View(turno);
            }

            // Validar que el turno esté dentro del horario de atención del profesional
            var horaTurno = turno.FechaHora.TimeOfDay; // 🔹 ahora es TimeSpan
            if (horaTurno < profesional.HoraInicio || horaTurno >= profesional.HoraFin)
            {
                ModelState.AddModelError("FechaHora",
                    $"El turno debe estar dentro del horario de atención del profesional ({profesional.HoraInicio:hh\\:mm} - {profesional.HoraFin:hh\\:mm}).");
                ViewData["PacienteId"] = new SelectList(_context.Pacientes, "Id", "NombreCompleto", turno.PacienteId);
                ViewData["ProfesionalId"] = new SelectList(_context.Profesionales, "Id", "NombreCompleto", turno.ProfesionalId);
                return View(turno);
            }

            // Validar que no haya superposición de turnos para el mismo profesional
            var duracionPrestacion = profesional.Prestacion.Duracion;
            var finTurnoNuevo = turno.FechaHora.AddMinutes(duracionPrestacion);

            var haySuperposicion = await _context.Turnos
                .Where(t => t.ProfesionalId == turno.ProfesionalId &&
                            t.Activo == true &&
                            t.Id != turno.Id &&
                            t.FechaHora.Date == turno.FechaHora.Date)
                .Include(t => t.Profesional)
                    .ThenInclude(p => p.Prestacion)
                .ToListAsync();

            foreach (var turnoExistente in haySuperposicion)
            {
                var finTurnoExistente = turnoExistente.FechaHora.AddMinutes(turnoExistente.Profesional.Prestacion.Duracion);

                // Chequear si hay superposición
                if (turno.FechaHora < finTurnoExistente && finTurnoNuevo > turnoExistente.FechaHora)
                {
                    ModelState.AddModelError("FechaHora",
                        $"El profesional ya tiene un turno asignado en ese horario (turno a las {turnoExistente.FechaHora:HH:mm}).");
                    ViewData["PacienteId"] = new SelectList(_context.Pacientes, "Id", "NombreCompleto", turno.PacienteId);
                    ViewData["ProfesionalId"] = new SelectList(_context.Profesionales, "Id", "NombreCompleto", turno.ProfesionalId);
                    return View(turno);
                }
            }


            if (ModelState.IsValid)
            {
                _context.Add(turno);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            ViewData["PacienteId"] = new SelectList(_context.Pacientes, "Id", "NombreCompleto", turno.PacienteId);
            ViewData["ProfesionalId"] = new SelectList(_context.Profesionales, "Id", "NombreCompleto", turno.ProfesionalId);
            return View(turno);
        }

        // GET: Turno/Edit/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var turno = await _context.Turnos
                .Include(t => t.Paciente)
                .Include(t => t.Profesional)
                .FirstOrDefaultAsync(t => t.Id == id);
            
            if (turno == null)
            {
                return NotFound();
            }
            
            ViewData["PacienteId"] = new SelectList(_context.Pacientes, "Id", "NombreCompleto", turno.PacienteId);
            ViewData["ProfesionalId"] = new SelectList(_context.Profesionales, "Id", "NombreCompleto", turno.ProfesionalId);
            return View(turno);
        }

        // POST: Turno/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FechaHora,FechaDeAlta,PacienteId,ProfesionalId,Confirmado,Activo,DescripcionCancelacion")] Turno turno)
        {
            if (id != turno.Id)
            {
                return NotFound();
            }

            // Si se cancela (Activo = false), se debe requerir descripción de cancelación
            if (!turno.Activo && string.IsNullOrWhiteSpace(turno.DescripcionCancelacion))
            {
                ModelState.AddModelError("DescripcionCancelacion", "Debe ingresar una descripción del motivo de cancelación.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(turno);
                    await _context.SaveChangesAsync();
                    
                    if (!turno.Activo)
                    {
                        TempData["Success"] = "Turno cancelado correctamente.";
                    }
                    else if (turno.Confirmado)
                    {
                        TempData["Success"] = "Turno confirmado correctamente.";
                    }
                    else
                    {
                        TempData["Success"] = "Turno actualizado correctamente.";
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TurnoExists(turno.Id))
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
            
            ViewData["PacienteId"] = new SelectList(_context.Pacientes, "Id", "NombreCompleto", turno.PacienteId);
            ViewData["ProfesionalId"] = new SelectList(_context.Profesionales, "Id", "NombreCompleto", turno.ProfesionalId);
            return View(turno);
        }

        // POST: Turno/Confirmar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Profesional")]
        public async Task<IActionResult> Confirmar(int id)
        {
            var turno = await _context.Turnos.FindAsync(id);
            if (turno == null)
            {
                return NotFound();
            }

            // Validar que si es profesional, sea SU turno
            if (User.IsInRole("Profesional"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (turno.ProfesionalId != currentUser.Id)
                {
                    return Forbid();
                }
            }

            turno.Confirmado = true;
            _context.Update(turno);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "✅ Turno confirmado exitosamente.";
            TempData["TipoMensaje"] = "success";

            return RedirectToAction(nameof(Index));
        }

        // GET: Turno/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var turno = await _context.Turnos
                .Include(t => t.Paciente)
                .Include(t => t.Profesional)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (turno == null)
            {
                return NotFound();
            }

            // Validar permisos: Paciente solo puede ver/cancelar sus propios turnos
            var currentUser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Paciente") && turno.PacienteId != currentUser.Id)
            {
                return Forbid();
            }
            // Profesional solo puede cancelar sus propios turnos
            if (User.IsInRole("Profesional") && turno.ProfesionalId != currentUser.Id)
            {
                return Forbid();
            }

            // Calcular si el paciente puede cancelar (regla de 24 horas)
            if (User.IsInRole("Paciente"))
            {
                var horasRestantes = (turno.FechaHora - DateTime.Now).TotalHours;
                ViewBag.PuedeCancelar = horasRestantes >= 24;
                ViewBag.HorasRestantes = horasRestantes;
            }
            else
            {
                ViewBag.PuedeCancelar = true;
            }

            return View(turno);
        }

        // POST: Turno/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var turno = await _context.Turnos.FindAsync(id);
            if (turno == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);

            // Validar permisos: Paciente solo puede cancelar sus propios turnos
            if (User.IsInRole("Paciente") && turno.PacienteId != currentUser.Id)
            {
                return Forbid();
            }
            // Profesional solo puede cancelar sus propios turnos
            if (User.IsInRole("Profesional") && turno.ProfesionalId != currentUser.Id)
            {
                return Forbid();
            }

            // Lógica de cancelación
            // Admin y Profesional pueden cancelar siempre
            // Paciente solo hasta 24hs antes

            if (User.IsInRole("Paciente"))
            {
                var horasRestantes = (turno.FechaHora - DateTime.Now).TotalHours;
                if (horasRestantes < 24)
                {
                    TempData["Mensaje"] = "❌ No puedes cancelar el turno con menos de 24 horas de anticipación.";
                    TempData["TipoMensaje"] = "danger";
                    return RedirectToAction(nameof(Index));
                }
            }

            // Eliminación física (podría ser lógica si tuviéramos campo Activo y quisiéramos historial)
            _context.Turnos.Remove(turno);
            await _context.SaveChangesAsync();
            
            TempData["Mensaje"] = "✅ Turno cancelado/eliminado exitosamente.";
            TempData["TipoMensaje"] = "success";
            
            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // WIZARD DE RESERVA DE TURNOS (EP5)
        // ==========================================

        // PASO 1: Selección de Prestación
        [Authorize(Roles = "Paciente")]
        public async Task<IActionResult> ReservarWizard()
        {
            // Validar si el paciente ya tiene un turno activo
            var currentUser = await _userManager.GetUserAsync(User);
            var tieneTurnoActivo = await _context.Turnos
                .AnyAsync(t => t.PacienteId == currentUser.Id && t.Activo == true && t.FechaHora > DateTime.Now);

            if (tieneTurnoActivo)
            {
                TempData["Mensaje"] = "⚠️ Ya tienes un turno activo. No puedes solicitar otro hasta completarlo o cancelarlo.";
                TempData["TipoMensaje"] = "warning";
                return RedirectToAction(nameof(Index));
            }

            var prestaciones = await _context.Prestaciones.ToListAsync();
            return View(prestaciones);
        }

        // PASO 2: Selección de Profesional
        [Authorize(Roles = "Paciente")]
        public async Task<IActionResult> ReservarWizardPaso2(int prestacionId)
        {
            var prestacion = await _context.Prestaciones.FindAsync(prestacionId);
            if (prestacion == null) return NotFound();

            var profesionales = await _context.Profesionales
                .Where(p => p.PrestacionId == prestacionId)
                .Include(p => p.Matricula)
                .ToListAsync();

            ViewBag.Prestacion = prestacion;
            return View(profesionales);
        }

        // PASO 3: Selección de Turno (Fecha y Hora)
        [Authorize(Roles = "Paciente")]
        public async Task<IActionResult> ReservarWizardPaso3(int prestacionId, string profesionalId)
        {
            var prestacion = await _context.Prestaciones.FindAsync(prestacionId);
            var profesional = await _context.Profesionales.FindAsync(profesionalId);

            if (prestacion == null || profesional == null) return NotFound();

            // Generar slots disponibles para los próximos 7 días
            var slots = new List<DateTime>();
            var hoy = DateTime.Now.Date;
            var duracion = prestacion.Duracion; // en minutos

            // Traer turnos ocupados del profesional en el rango
            var turnosOcupados = await _context.Turnos
                .Where(t => t.ProfesionalId == profesionalId && 
                            t.Activo && 
                            t.FechaHora >= hoy && 
                            t.FechaHora <= hoy.AddDays(8)) // un poco más de margen
                .Select(t => t.FechaHora)
                .ToListAsync();

            // Iterar 7 días
            for (int i = 0; i < 7; i++)
            {
                var fecha = hoy.AddDays(i);
                
                // Solo lunes a viernes
                if (fecha.DayOfWeek == DayOfWeek.Saturday || fecha.DayOfWeek == DayOfWeek.Sunday)
                    continue;

                // Definir hora inicio y fin del día para el profesional
                // Usamos la fecha actual + la hora del profesional
                var horaInicio = fecha.Add(profesional.HoraInicio);
                var horaFin = fecha.Add(profesional.HoraFin);

                var slotActual = horaInicio;

                while (slotActual.AddMinutes(duracion) <= horaFin)
                {
                    // Validar que sea futuro
                    if (slotActual > DateTime.Now)
                    {
                        // Validar colisión
                        // Un turno ocupa [slotActual, slotActual + duracion]
                        // Chequeamos si algun turno ocupado cae en este rango
                        // Simplificación: asumimos que los turnos ocupados arrancan exactamente en los slots
                        // Pero para ser robustos:
                        bool ocupado = turnosOcupados.Any(t => 
                            Math.Abs((t - slotActual).TotalMinutes) < duracion // Si hay un turno que arranca a menos de 'duracion' minutos de diferencia, hay solapamiento
                        );

                        if (!ocupado)
                        {
                            slots.Add(slotActual);
                        }
                    }
                    slotActual = slotActual.AddMinutes(duracion);
                }
            }

            ViewBag.Prestacion = prestacion;
            ViewBag.Profesional = profesional;
            return View(slots);
        }

        // PASO 4: Confirmación (POST)
        [HttpPost]
        [Authorize(Roles = "Paciente")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarReserva(int prestacionId, string profesionalId, DateTime fechaHora)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            
            // Re-validaciones de seguridad
            var tieneTurno = await _context.Turnos
                .AnyAsync(t => t.PacienteId == currentUser.Id && t.Activo && t.FechaHora > DateTime.Now);
            
            if (tieneTurno) return RedirectToAction(nameof(Index));

            var turno = new Turno
            {
                FechaHora = fechaHora,
                FechaDeAlta = DateTime.Now,
                PacienteId = currentUser.Id,
                ProfesionalId = profesionalId,
                Confirmado = false,
                Activo = true
            };

            _context.Add(turno);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "✅ ¡Turno reservado con éxito!";
            TempData["TipoMensaje"] = "success";

            return RedirectToAction(nameof(Index));
        }

        private bool TurnoExists(int id)
        {
            return _context.Turnos.Any(e => e.Id == id);
        }
    }
}
