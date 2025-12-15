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

namespace Agenda_C.Controllers
{
    // NOTA: Renombramos todo a plural porque el profe marcó que solo PacientesController
    // estaba bien y el resto estaba en singular. Ahora todos en plural para ser consistentes.
    public class PrestacionesController : Controller
    {
        private readonly AgendaContext _context;

        public PrestacionesController(AgendaContext context)
        {
            _context = context;
        }

        // GET: Prestacion
        // CAMBIO EP4: Dejamos esto público para que cualquiera pueda ver qué ofrecemos (Checklist: "Listar prestaciones... sin iniciar sesión")
        public async Task<IActionResult> Index()
        {
            return View(await _context.Prestaciones.ToListAsync());
        }

        // GET: Prestacion/Details/5
        // CAMBIO EP4: También público para ver detalles
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prestacion = await _context.Prestaciones
                .Include(p => p.Profesionales)
                    .ThenInclude(prof => prof.Matricula)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (prestacion == null)
            {
                return NotFound();
            }

            return View(prestacion);
        }

        // GET: Prestacion/Create
        // CAMBIO EP4: Solo Admin puede crear prestaciones nuevas
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Prestacion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Descripcion,Duracion,Precio")] Prestacion prestacion)
        {
            // Validar que el nombre sea único
            var nombreExiste = await _context.Prestaciones
                .AnyAsync(p => p.Nombre == prestacion.Nombre);
            
            if (nombreExiste)
            {
                ModelState.AddModelError("Nombre", "Ya existe una prestación con ese nombre. Debe ser único.");
                return View(prestacion);
            }

            if (ModelState.IsValid)
            {
                _context.Add(prestacion);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Prestación creada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(prestacion);
        }

        // GET: Prestacion/Edit/5
        // CAMBIO EP4: Solo Admin puede editar
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prestacion = await _context.Prestaciones.FindAsync(id);
            if (prestacion == null)
            {
                return NotFound();
            }
            return View(prestacion);
        }

        // POST: Prestacion/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion,Duracion,Precio")] Prestacion prestacion)
        {
            if (id != prestacion.Id)
            {
                return NotFound();
            }

            // Validar que el nombre sea único (excepto el mismo registro)
            var nombreExiste = await _context.Prestaciones
                .AnyAsync(p => p.Nombre == prestacion.Nombre && p.Id != prestacion.Id);
            
            if (nombreExiste)
            {
                ModelState.AddModelError("Nombre", "Ya existe otra prestación con ese nombre. Debe ser único.");
                return View(prestacion);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(prestacion);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Prestación actualizada correctamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrestacionExists(prestacion.Id))
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
            return View(prestacion);
        }

        // GET: Prestacion/Delete/5
        // CAMBIO EP4: Solo Admin puede borrar
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prestacion = await _context.Prestaciones
                .FirstOrDefaultAsync(m => m.Id == id);
            if (prestacion == null)
            {
                return NotFound();
            }

            return View(prestacion);
        }

        // POST: Prestacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var prestacion = await _context.Prestaciones
                .Include(p => p.Profesionales)
                .FirstOrDefaultAsync(p => p.Id == id);
            
            if (prestacion != null)
            {
                // Verificar que no tenga profesionales asociados
                if (prestacion.Profesionales != null && prestacion.Profesionales.Any())
                {
                    TempData["Error"] = "No se puede eliminar una prestación que tiene profesionales asignados. Primero debe reasignar o eliminar los profesionales.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Prestaciones.Remove(prestacion);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Prestación eliminada correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PrestacionExists(int id)
        {
            return _context.Prestaciones.Any(e => e.Id == id);
        }
    }
}
