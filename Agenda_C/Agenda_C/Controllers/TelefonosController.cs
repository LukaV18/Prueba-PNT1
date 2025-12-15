using System;
using System.Linq;
using System.Text.RegularExpressions;
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
    [Authorize(Roles = "Administrador,Paciente,Profesional")]
    public class TelefonosController : Controller
    {
        private readonly AgendaContext _context;

        public TelefonosController(AgendaContext context)
        {
            _context = context;
        }

        // GET: Telefono
        public async Task<IActionResult> Index()
        {
            var telefonos = await _context.Telefonos
                .Include(t => t.Persona)
                .OrderBy(t => t.Persona.Apellido)
                .ThenBy(t => t.Persona.Nombre)
                .ToListAsync();

            return View(telefonos);
        }

        // GET: Telefono/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var telefono = await _context.Telefonos
                .Include(t => t.Persona)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (telefono == null)
            {
                return NotFound();
            }

            return View(telefono);
        }

        // GET: Telefono/Create
        public IActionResult Create()
        {
            // NOTA: El DbSet se llama "Persona" (singular) no "Personas"
            ViewData["PersonaId"] = new SelectList(_context.Persona, "Id", "NombreCompleto");
            ViewData["TiposDeTelefono"] = new SelectList(Enum.GetValues(typeof(TipoDeTelefono)));
            return View();
        }

        // POST: Telefono/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CodigoPais,Prefijo,Numero,TipoDeTelefono,PersonaId")] Telefono telefono)
        {
            // Normalizar entradas
            if (!string.IsNullOrEmpty(telefono.CodigoPais))
                telefono.CodigoPais = telefono.CodigoPais.Trim();

            if (!string.IsNullOrEmpty(telefono.Prefijo))
                telefono.Prefijo = telefono.Prefijo.Trim();

            if (!string.IsNullOrEmpty(telefono.Numero))
                telefono.Numero = telefono.Numero.Trim();

            // Validaciones
            if (!ValidarCodigoPais(telefono.CodigoPais))
            {
                ModelState.AddModelError("CodigoPais",
                    "El código de país debe tener entre 1 y 4 dígitos. Ejemplos: +54, 54, +1, 1");
            }

            if (!ValidarPrefijo(telefono.Prefijo))
            {
                ModelState.AddModelError("Prefijo",
                    "El prefijo debe tener entre 2 y 5 dígitos. Ejemplos: 11, 341, 0351");
            }
            if (!ValidarNumero(telefono.Numero))
            {
                ModelState.AddModelError("Numero",
                    "El número debe tener entre 6 y 10 dígitos, sin espacios ni guiones. Ejemplo: 12345678");
            }

            var longitudTotal = (telefono.CodigoPais?.Replace("+", "").Length ?? 0) +
                                (telefono.Prefijo?.Length ?? 0) +
                                (telefono.Numero?.Length ?? 0);

            if (longitudTotal > 18)
            {
                ModelState.AddModelError("",
                    "La longitud total del teléfono (código país + prefijo + número) no debe superar los 18 dígitos.");
            }

            // Validar duplicados
            if (!string.IsNullOrWhiteSpace(telefono.PersonaId))
            {
                var telefonoDuplicado = await _context.Telefonos
                    .AnyAsync(t => t.PersonaId == telefono.PersonaId &&
                                   t.CodigoPais == telefono.CodigoPais &&
                                   t.Prefijo == telefono.Prefijo &&
                                   t.Numero == telefono.Numero);

                if (telefonoDuplicado)
                {
                     ModelState.AddModelError("",
                        "Este teléfono ya está registrado para esta persona.");
                }
            }

            // Guardar si todo es válido
            if (ModelState.IsValid)
            {
                _context.Add(telefono);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "✅ Teléfono creado exitosamente.";
                TempData["TipoMensaje"] = "success";

                return RedirectToAction(nameof(Index));
            }

            // Si hay errores, volver a la vista con los combos cargados
            ViewData["PersonaId"] = new SelectList(_context.Persona, "Id", "NombreCompleto", telefono.PersonaId);
            ViewData["TiposDeTelefono"] = new SelectList(Enum.GetValues(typeof(TipoDeTelefono)), telefono.TipoDeTelefono);
            return View(telefono);
        }

        // GET: Telefono/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var telefono = await _context.Telefonos.FindAsync(id);
            if (telefono == null)
            {
                return NotFound();
            }

            ViewData["PersonaId"] = new SelectList(_context.Persona, "Id", "NombreCompleto", telefono.PersonaId);
            ViewData["TiposDeTelefono"] = new SelectList(Enum.GetValues(typeof(TipoDeTelefono)), telefono.TipoDeTelefono);
            return View(telefono);
        }

        // POST: Telefono/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CodigoPais,Prefijo,Numero,TipoDeTelefono,PersonaId")] Telefono telefono)
        {
            var existe = await _context.Telefonos.AnyAsync(t => t.Id == id);
            if (!existe)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(telefono.CodigoPais))
                telefono.CodigoPais = telefono.CodigoPais.Trim();

            if (!string.IsNullOrEmpty(telefono.Prefijo))
                telefono.Prefijo = telefono.Prefijo.Trim();

            if (!string.IsNullOrEmpty(telefono.Numero))
                telefono.Numero = telefono.Numero.Trim();

            
            if (!ValidarCodigoPais(telefono.CodigoPais))
            {
                ModelState.AddModelError("CodigoPais",
                    "El código de país debe tener entre 1 y 4 dígitos. Ejemplos: +54, 54, +1, 1");
            }

            if (!ValidarPrefijo(telefono.Prefijo))
            {
                ModelState.AddModelError("Prefijo",
                    "El prefijo debe tener entre 2 y 5 dígitos. Ejemplos: 11, 341, 0351");
            }

            
            if (!ValidarNumero(telefono.Numero))
            {
                ModelState.AddModelError("Numero",
                    "El número debe tener entre 6 y 10 dígitos, sin espacios ni guiones. Ejemplo: 12345678");
            }

            var longitudTotal = (telefono.CodigoPais?.Replace("+", "").Length ?? 0) +
                               (telefono.Prefijo?.Length ?? 0) +
                               (telefono.Numero?.Length ?? 0);

            if (longitudTotal > 18)
            {
                ModelState.AddModelError("",
                    "La longitud total del teléfono no debe superar los 18 dígitos.");
            }

            if (!string.IsNullOrWhiteSpace(telefono.PersonaId))

            {
                var telefonoDuplicado = await _context.Telefonos
                    .AnyAsync(t => t.Id != telefono.Id &&
                                  t.PersonaId == telefono.PersonaId &&
                                  t.CodigoPais == telefono.CodigoPais &&
                                  t.Prefijo == telefono.Prefijo &&
                                  t.Numero == telefono.Numero);

                if (telefonoDuplicado)
                {
                    ModelState.AddModelError("",
                        "Este teléfono ya está registrado para esta persona.");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(telefono);
                    await _context.SaveChangesAsync();

                    TempData["Mensaje"] = "✅ Teléfono actualizado exitosamente.";
                    TempData["TipoMensaje"] = "success";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Telefonos.AnyAsync(t => t.Id == id))
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

            ViewData["PersonaId"] = new SelectList(_context.Persona, "Id", "NombreCompleto", telefono.PersonaId);
            ViewData["TiposDeTelefono"] = new SelectList(Enum.GetValues(typeof(TipoDeTelefono)), telefono.TipoDeTelefono);
            return View(telefono);
        }

        // GET: Telefono/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var telefono = await _context.Telefonos
                .Include(t => t.Persona)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (telefono == null)
            {
                return NotFound();
            }

            return View(telefono);
        }

        // POST: Telefono/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var telefono = await _context.Telefonos.FindAsync(id);
            if (telefono != null)
            {
                _context.Telefonos.Remove(telefono);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "✅ Teléfono eliminado exitosamente.";
                TempData["TipoMensaje"] = "success";
            }

            return RedirectToAction(nameof(Index));
        }

 
        private bool ValidarCodigoPais(string codigoPais)
        {
            if (string.IsNullOrWhiteSpace(codigoPais))
                return false;

            // Permitir + al inicio, seguido de 1-4 dígitos
            var regex = new Regex(@"^\+?\d{1,4}$");
            return regex.IsMatch(codigoPais);
        }

  
        private bool ValidarPrefijo(string prefijo)
        {
            if (string.IsNullOrWhiteSpace(prefijo))
                return false;

            
            var regex = new Regex(@"^\d{2,5}$");
            return regex.IsMatch(prefijo);
        }

     
        private bool ValidarNumero(string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
                return false;

           
            var regex = new Regex(@"^\d{6,10}$");
            return regex.IsMatch(numero);
        }

        private async Task<bool> TelefonoExists(int id)
        {
            return await _context.Telefonos.AnyAsync(e => e.Id == id);   // ✔ await aplicado
        }
    }
}
