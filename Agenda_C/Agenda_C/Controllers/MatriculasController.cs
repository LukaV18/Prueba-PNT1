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

namespace Agenda_C.Controllers
{
    // NOTA: Renombramos todo a plural porque el profe marcó que solo PacientesController
    // estaba bien y el resto estaba en singular. Ahora todos en plural para ser consistentes.
    [Authorize(Roles = "Administrador")]
    public class MatriculasController : Controller
    {
        private readonly AgendaContext _context;

        public MatriculasController(AgendaContext context)
        {
            _context = context;
        }

        // GET: Matricula
        public async Task<IActionResult> Index()
        {
            // Ordenamos por tipo y luego por número para que sea más organizado
            var matriculas = await _context.Matriculas
                .Include(m => m.Profesional)
                .OrderBy(m => m.Tipo)
                .ThenBy(m => m.Provincia)
                .ThenBy(m => m.Numero)
                .ToListAsync();

            return View(matriculas);
        }

        // GET: Matricula/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var matricula = await _context.Matriculas
                .Include(m => m.Profesional)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (matricula == null)
            {
                return NotFound();
            }

            return View(matricula);
        }

        // GET: Matricula/Create
        public IActionResult Create()
        {
            ViewData["Provincias"] = new SelectList(Enum.GetValues(typeof(Provincia)));
            ViewData["TiposMatricula"] = new SelectList(Enum.GetValues(typeof(TipoMatricula)));
            return View();
        }

        // POST: Matricula/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Numero,Provincia,Tipo")] Matricula matricula)
        {
            // VALIDACIÓN CRÍTICA: Usamos el helper para verificar unicidad de Numero+Tipo+Provincia
            var existeMatricula = await ValidationHelper.MatriculaExisteAsync(
                _context, 
                matricula.Numero, 
                matricula.Tipo, 
                matricula.Provincia
            );

            if (existeMatricula)
            {
                ModelState.AddModelError("", 
                    $"Ya existe una matrícula {matricula.Tipo} N° {matricula.Numero} en {matricula.Provincia}. " +
                    "Cada matrícula debe ser única por tipo y provincia.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(matricula);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "✅ Matrícula creada exitosamente.";
                TempData["TipoMensaje"] = "success";

                return RedirectToAction(nameof(Index));
            }

            ViewData["Provincias"] = new SelectList(Enum.GetValues(typeof(Provincia)), matricula.Provincia);
            ViewData["TiposMatricula"] = new SelectList(Enum.GetValues(typeof(TipoMatricula)), matricula.Tipo);
            return View(matricula);
        }

        // GET: Matricula/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var matricula = await _context.Matriculas.FindAsync(id);
            if (matricula == null)
            {
                return NotFound();
            }

            ViewData["Provincias"] = new SelectList(Enum.GetValues(typeof(Provincia)), matricula.Provincia);
            ViewData["TiposMatricula"] = new SelectList(Enum.GetValues(typeof(TipoMatricula)), matricula.Tipo);
            return View(matricula);
        }

        // POST: Matricula/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Numero,Provincia,Tipo")] Matricula matricula)
        {
            if (id != matricula.Id)
            {
                return NotFound();
            }

            // Usamos el helper para validar unicidad excluyendo la matrícula actual
            var existeMatricula = await ValidationHelper.MatriculaExisteAsync(
                _context, 
                matricula.Numero, 
                matricula.Tipo, 
                matricula.Provincia,
                id
            );

            if (existeMatricula)
            {
                ModelState.AddModelError("", 
                    $"Ya existe otra matrícula {matricula.Tipo} N° {matricula.Numero} en {matricula.Provincia}.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(matricula);
                    await _context.SaveChangesAsync();

                    TempData["Mensaje"] = "✅ Matrícula actualizada correctamente.";
                    TempData["TipoMensaje"] = "success";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MatriculaExists(matricula.Id))
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

            ViewData["Provincias"] = new SelectList(Enum.GetValues(typeof(Provincia)), matricula.Provincia);
            ViewData["TiposMatricula"] = new SelectList(Enum.GetValues(typeof(TipoMatricula)), matricula.Tipo);
            return View(matricula);
        }

        // GET: Matricula/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var matricula = await _context.Matriculas
                .Include(m => m.Profesional)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (matricula == null)
            {
                return NotFound();
            }

            // Usamos el helper para verificar si tiene profesional asignado
            ViewBag.TieneProfesional = await ValidationHelper.MatriculaTieneProfesionalAsync(_context, id.Value);

            return View(matricula);
        }

        // POST: Matricula/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var matricula = await _context.Matriculas
                .Include(m => m.Profesional)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (matricula == null)
            {
                return NotFound();
            }

            // VALIDACIÓN: Usamos el helper para verificar si está asignada a un profesional
            if (await ValidationHelper.MatriculaTieneProfesionalAsync(_context, id))
            {
                TempData["Mensaje"] = "❌ No se puede eliminar esta matrícula porque está asignada a un profesional. " +
                                     "Primero hay que desvincularla o eliminar el profesional.";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Matriculas.Remove(matricula);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "✅ Matrícula eliminada exitosamente.";
                TempData["TipoMensaje"] = "success";
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"❌ Error al eliminar la matrícula: {ex.Message}";
                TempData["TipoMensaje"] = "danger";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool MatriculaExists(int id)
        {
            return _context.Matriculas.Any(e => e.Id == id);
        }
    }
}
