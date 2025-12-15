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
using Agenda_C.Helpers;

namespace Agenda_C.Controllers
{
    // NOTA: Renombramos todo a plural porque el profe marcó que solo PacientesController
    // estaba bien y el resto estaba en singular. Ahora todos en plural para ser consistentes.
    [Authorize(Roles = "Administrador")]
    public class CoberturasController : Controller
    {
        private readonly AgendaContext _context;

        public CoberturasController(AgendaContext context)
        {
            _context = context;
        }

        // GET: Cobertura
        public async Task<IActionResult> Index()
        {
            var coberturas = await _context.Coberturas
                .Include(c => c.Paciente)
                .OrderBy(c => c.Prestadora)
                .ThenBy(c => c.NumeroCredencial)
                .ToListAsync();

            return View(coberturas);
        }

        // GET: Cobertura/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cobertura = await _context.Coberturas
                .Include(c => c.Paciente)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cobertura == null)
            {
                return NotFound();
            }

            return View(cobertura);
        }

        // GET: Cobertura/Create
        public IActionResult Create()
        {
            ViewData["PacienteId"] = new SelectList(_context.Pacientes, "Id", "NombreCompleto");
            ViewData["Prestadoras"] = new SelectList(Enum.GetValues(typeof(Prestadora)));
            return View();
        }

        // POST: Cobertura/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NumeroCredencial,Prestadora,PacienteId")] Cobertura cobertura)
        {
            // ✅ VALIDACIÓN 1: Usamos el helper para verificar que NumeroCredencial sea único
            if (await ValidationHelper.NumeroCredencialExisteAsync(_context, cobertura.NumeroCredencial))
            {
                ModelState.AddModelError("NumeroCredencial",
                    $"El número de credencial '{cobertura.NumeroCredencial}' ya está registrado en el sistema.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(cobertura);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "✅ Cobertura creada exitosamente.";
                TempData["TipoMensaje"] = "success";

                return RedirectToAction(nameof(Index));
            }

            ViewData["PacienteId"] = new SelectList(_context.Pacientes, "Id", "NombreCompleto", cobertura.PacienteId);
            ViewData["Prestadoras"] = new SelectList(Enum.GetValues(typeof(Prestadora)), cobertura.Prestadora);
            return View(cobertura);
        }

        // GET: Cobertura/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cobertura = await _context.Coberturas.FindAsync(id);
            if (cobertura == null)
            {
                return NotFound();
            }

            ViewData["PacienteId"] = new SelectList(_context.Pacientes, "Id", "NombreCompleto", cobertura.PacienteId);
            ViewData["Prestadoras"] = new SelectList(Enum.GetValues(typeof(Prestadora)), cobertura.Prestadora);
            return View(cobertura);
        }

        // POST: Cobertura/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NumeroCredencial,Prestadora,PacienteId")] Cobertura cobertura)
        {
            if (id != cobertura.Id)
            {
                return NotFound();
            }

            // Usamos el helper para validar unicidad excluyendo la cobertura actual
            if (await ValidationHelper.NumeroCredencialExisteAsync(_context, cobertura.NumeroCredencial, cobertura.Id))
            {
                ModelState.AddModelError("NumeroCredencial",
                    $"El número de credencial '{cobertura.NumeroCredencial}' ya está registrado en el sistema.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cobertura);
                    await _context.SaveChangesAsync();

                    TempData["Mensaje"] = "✅ Cobertura actualizada exitosamente.";
                    TempData["TipoMensaje"] = "success";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CoberturaExists(cobertura.Id))
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

            ViewData["PacienteId"] = new SelectList(_context.Pacientes, "Id", "NombreCompleto", cobertura.PacienteId);
            ViewData["Prestadoras"] = new SelectList(Enum.GetValues(typeof(Prestadora)), cobertura.Prestadora);
            return View(cobertura);
        }

        // GET: Cobertura/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cobertura = await _context.Coberturas
                .Include(c => c.Paciente)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cobertura == null)
            {
                return NotFound();
            }

           
            var tienePacientes = await TienePacientesAsociados(id.Value);
            ViewBag.TienePacientes = tienePacientes;

           
            var tieneFormularios = await _context.Formularios.AnyAsync(f => f.CoberturaId == id);
            ViewBag.TieneFormularios = tieneFormularios;

            return View(cobertura);
        }

        // POST: Cobertura/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cobertura = await _context.Coberturas
                .Include(c => c.Paciente)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cobertura == null)
            {
                return NotFound();
            }

           
            if (await TienePacientesAsociados(id))
            {
                TempData["Mensaje"] = "❌ No se puede eliminar esta cobertura porque tiene pacientes asociados.";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction(nameof(Index));
            }

          
            var tieneFormularios = await _context.Formularios.AnyAsync(f => f.CoberturaId == id);
            if (tieneFormularios)
            {
                TempData["Mensaje"] = "❌ No se puede eliminar esta cobertura porque tiene formularios asociados.";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Coberturas.Remove(cobertura);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "✅ Cobertura eliminada exitosamente.";
                TempData["TipoMensaje"] = "success";
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"❌ Error al eliminar la cobertura: {ex.Message}";
                TempData["TipoMensaje"] = "danger";
            }

            return RedirectToAction(nameof(Index));
        }

        // Usamos el helper para verificar si tiene pacientes asociados
        private async Task<bool> TienePacientesAsociados(int coberturaId)
        {
            return await ValidationHelper.CoberturaTienePacientesAsync(_context, coberturaId);
        }

        private bool CoberturaExists(int id)
        {
            return _context.Coberturas.Any(e => e.Id == id);
        }
    }
}
