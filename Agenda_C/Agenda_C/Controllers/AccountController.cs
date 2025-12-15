using Agenda_C.Models;
using Agenda_C.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Agenda_C.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Persona> _userManager;
        private readonly SignInManager<Persona> _signInManager;

        public AccountController(
            UserManager<Persona> userManager,
            SignInManager<Persona> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Account/Login
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                // Intentar login
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    // Redirigir según returnUrl o al Home
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }

                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty,
                        "La cuenta está bloqueada por múltiples intentos fallidos. Intente más tarde.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty,
                        "Credenciales inválidas. Verifique su email y contraseña.");
                }
            }

            return View(model);
        }

        // POST: Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // GET: Account/Register (solo para Pacientes)
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Crear el Paciente
                var paciente = new Paciente
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Nombre = model.Nombre,
                    Apellido = model.Apellido,
                    DNI = model.DNI,
                    FechaAlta = DateTime.Now
                };

                var result = await _userManager.CreateAsync(paciente, model.Password);

                if (result.Succeeded)
                {
                    // Asignar rol Paciente
                    await _userManager.AddToRoleAsync(paciente, "Paciente");

                    // Login automático
                    await _signInManager.SignInAsync(paciente, isPersistent: false);

                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        // GET: Account/AccessDenied
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
   

