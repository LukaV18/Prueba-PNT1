using Agenda_C.Data;
using Agenda_C.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace Agenda_C
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ============= CONFIGURACIÓN ENTITY FRAMEWORK (EP4 - SQL SERVER) =============
            // CAMBIO EP4: Chicos, cambié a SQL Server porque el checklist lo pide explícitamente.
            // "Persistencia en SQL LocalDB". Acuérdense de correr 'update-database' si les tira error.
            builder.Services.AddDbContext<AgendaContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("AgendaContext")));

            // Configuración de Identity
            builder.Services.AddIdentity<Persona, IdentityRole>(options =>
            {
                // Password settings (para desarrollo - luego ajustar en producción)
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;

                // Lockout settings (bloqueo por intentos fallidos)
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            })
       .AddEntityFrameworkStores<AgendaContext>()
       .AddDefaultTokenProviders();

            // Configurar la ruta de login y acceso denegado
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.LogoutPath = "/Account/Logout";
            });

            builder.Services.AddControllersWithViews();

            var app = builder.Build();
            // Usar Authentication y Authorization
            app.UseAuthentication();  // ← AGREGAR ANTES de UseAuthorization
            app.UseAuthorization();

            // ============= SEED DATA =============
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await SeedData.Initialize(services);
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
