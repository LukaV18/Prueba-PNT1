using Agenda_C.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Agenda_C.Data
{
    public static class SeedData
    {


        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<AgendaContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<Persona>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Asegurar que la base de datos esté creada y las migraciones aplicadas
            await context.Database.MigrateAsync();

            // 1. Crear Roles
            string[] roleNames = { "Administrador", "Profesional", "Paciente" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 2. Crear Administrador por defecto
            var adminUser = await userManager.FindByEmailAsync("admin1@ort.edu.ar");
            if (adminUser == null)
            {
                // Usamos la entidad concreta Administrador
                var admin = new Administrador
                {
                    UserName = "admin1@ort.edu.ar",
                    Email = "admin1@ort.edu.ar",
                    NameUser = "admin1",
                    EmailUser = "admin1@ort.edu.ar",
                    Nombre = "Administrador",
                    Apellido = "Sistema",
                    DNI = 11111111,
                    FechaAlta = DateTime.Now,
                    Cargo = "Administrador General"
                };

                var result = await userManager.CreateAsync(admin, "Password1!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Administrador");
                }
            }

            // ============= SEED PRESTACIONES =============
            if (!context.Prestaciones.Any())
            {
                var prestaciones = new List<Prestacion>
                {
                    new Prestacion
                    {
                        Nombre = "Consulta General",
                        Descripcion = "Consulta médica general de rutina",
                        Duracion = 30,
                        Precio = 5000.00m
                    },
                    new Prestacion
                    {
                        Nombre = "Consulta Cardiología",
                        Descripcion = "Consulta especializada en cardiología",
                        Duracion = 45,
                        Precio = 8000.00m
                    },
                    new Prestacion
                    {
                        Nombre = "Consulta Pediatría",
                        Descripcion = "Consulta pediátrica para niños y adolescentes",
                        Duracion = 30,
                        Precio = 6000.00m
                    },
                    new Prestacion
                    {
                        Nombre = "Consulta Dermatología",
                        Descripcion = "Consulta especializada en dermatología",
                        Duracion = 30,
                        Precio = 7000.00m
                    },
                    new Prestacion
                    {
                        Nombre = "Consulta Traumatología",
                        Descripcion = "Consulta especializada en traumatología y ortopedia",
                        Duracion = 45,
                        Precio = 8500.00m
                    }
                };
                context.Prestaciones.AddRange(prestaciones);
                context.SaveChanges();
            }

            // ============= SEED MATRICULAS =============
            if (!context.Matriculas.Any())
            {
                var matriculas = new List<Matricula>
                {
                    new Matricula { Numero = 12345, Provincia = Provincia.BuenosAires, Tipo = TipoMatricula.MN },
                    new Matricula { Numero = 23456, Provincia = Provincia.Cordoba, Tipo = TipoMatricula.MP },
                    new Matricula { Numero = 34567, Provincia = Provincia.SantaFe, Tipo = TipoMatricula.MN },
                    new Matricula { Numero = 45678, Provincia = Provincia.BuenosAires, Tipo = TipoMatricula.MP },
                    new Matricula { Numero = 56789, Provincia = Provincia.Mendoza, Tipo = TipoMatricula.MN }
                };
                context.Matriculas.AddRange(matriculas);
                context.SaveChanges();
            }

            // ============= SEED COBERTURAS =============
            if (!context.Coberturas.Any())
            {
                var coberturas = new List<Cobertura>
                {
                    new Cobertura { NumeroCredencial = 111111, Prestadora = Prestadora.OSDE },
                    new Cobertura { NumeroCredencial = 222222, Prestadora = Prestadora.SwissMedical },
                    new Cobertura { NumeroCredencial = 333333, Prestadora = Prestadora.Galeno },
                    new Cobertura { NumeroCredencial = 444444, Prestadora = Prestadora.SancorSalud },
                    new Cobertura { NumeroCredencial = 555555, Prestadora = Prestadora.OSDE },
                    new Cobertura { NumeroCredencial = 666666, Prestadora = Prestadora.SwissMedical }
                };
                context.Coberturas.AddRange(coberturas);
                context.SaveChanges();
            }

            // ============= SEED DIRECCIONES =============
            if (!context.Direcciones.Any())
            {
                var direcciones = new List<Direccion>
                {
                    new Direccion { Calle = "Av. Corrientes", Altura = 1234, Piso = 5, Departamento = "A" },
                    new Direccion { Calle = "Av. Santa Fe", Altura = 2345, Piso = null, Departamento = null },
                    new Direccion { Calle = "Calle Florida", Altura = 3456, Piso = 10, Departamento = "B" },
                    new Direccion { Calle = "Av. Callao", Altura = 4567, Piso = 2, Departamento = "C" },
                    new Direccion { Calle = "Av. Rivadavia", Altura = 5678, Piso = 8, Departamento = "D" },
                    new Direccion { Calle = "Calle Lavalle", Altura = 6789, Piso = null, Departamento = null }
                };
                context.Direcciones.AddRange(direcciones);
                context.SaveChanges();
            }

            // ============= SEED PACIENTES =============
            if (!context.Pacientes.Any())
            {
                var coberturas = context.Coberturas.ToList();
                var direcciones = context.Direcciones.ToList();

                var pacientes = new List<Paciente>
                {
                    new Paciente
                    {
                        UserName = "paciente1@ort.edu.ar", // Identity requiere email como username a veces, o coincidir
                        Email = "paciente1@ort.edu.ar",
                        NameUser = "paciente1",
                        EmailUser = "paciente1@ort.edu.ar",
                        Nombre = "Juan",
                        Apellido = "Pérez",
                        DNI = 12345678,
                        CoberturaId = coberturas[0].Id,
                        Direccion = direcciones[0],
                        FechaAlta = DateTime.Now
                    },
                    new Paciente
                    {
                        UserName = "paciente2@ort.edu.ar",
                        Email = "paciente2@ort.edu.ar",
                        NameUser = "paciente2",
                        EmailUser = "paciente2@ort.edu.ar",
                        Nombre = "María",
                        Apellido = "González",
                        DNI = 23456789,
                        CoberturaId = coberturas[1].Id,
                        Direccion = direcciones[1],
                        FechaAlta = DateTime.Now
                    },
                    new Paciente
                    {
                        UserName = "paciente3@ort.edu.ar",
                        Email = "paciente3@ort.edu.ar",
                        NameUser = "paciente3",
                        EmailUser = "paciente3@ort.edu.ar",
                        Nombre = "Carlos",
                        Apellido = "Rodríguez",
                        DNI = 34567890,
                        CoberturaId = coberturas[2].Id,
                        Direccion = direcciones[2],
                        FechaAlta = DateTime.Now
                    },
                    new Paciente
                    {
                        UserName = "paciente4@ort.edu.ar",
                        Email = "paciente4@ort.edu.ar",
                        NameUser = "paciente4",
                        EmailUser = "paciente4@ort.edu.ar",
                        Nombre = "Ana",
                        Apellido = "Martínez",
                        DNI = 45678901,
                        CoberturaId = coberturas[3].Id,
                        Direccion = direcciones[3],
                        FechaAlta = DateTime.Now
                    }
                };

                foreach (var paciente in pacientes)
                {
                    var result = await userManager.CreateAsync(paciente, "Password1!");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(paciente, "Paciente");
                    }
                }
            }

            // ============= SEED PROFESIONALES =============
            if (!context.Profesionales.Any())
            {
                var matriculas = context.Matriculas.ToList();
                var prestaciones = context.Prestaciones.ToList();
                var direcciones = context.Direcciones.ToList();

                var profesionales = new List<Profesional>
                {
                    new Profesional
                    {
                        UserName = "profesional1@ort.edu.ar",
                        Email = "profesional1@ort.edu.ar",
                        NameUser = "profesional1",
                        EmailUser = "profesional1@ort.edu.ar",
                        Nombre = "Dr. Roberto",
                        Apellido = "Martínez",
                        DNI = 20123456,
                        MatriculaId = matriculas[0].Id,
                        PrestacionId = prestaciones[0].Id,
                        HoraInicio = new TimeSpan(8, 0, 0),
                        HoraFin = new TimeSpan(16, 0, 0),
                        Direccion = direcciones[4],
                        FechaAlta = DateTime.Now
                    },
                    new Profesional
                    {
                        UserName = "profesional2@ort.edu.ar",
                        Email = "profesional2@ort.edu.ar",
                        NameUser = "profesional2",
                        EmailUser = "profesional2@ort.edu.ar",
                        Nombre = "Dra. Ana",
                        Apellido = "López",
                        DNI = 20234567,
                        MatriculaId = matriculas[1].Id,
                        PrestacionId = prestaciones[1].Id,
                        HoraInicio = new TimeSpan(9, 0,0),
                        HoraFin = new TimeSpan(17, 0,0),
                        Direccion = direcciones[4],
                        FechaAlta = DateTime.Now
                    },
                    new Profesional
                    {
                        UserName = "profesional3@ort.edu.ar",
                        Email = "profesional3@ort.edu.ar",
                        NameUser = "profesional3",
                        EmailUser = "profesional3@ort.edu.ar",
                        Nombre = "Dr. Pedro",
                        Apellido = "Fernández",
                        DNI = 20345678,
                        MatriculaId = matriculas[2].Id,
                        PrestacionId = prestaciones[2].Id,
                        HoraInicio = new TimeSpan(8, 30,0),
                        HoraFin = new TimeSpan(16, 30,0),
                        Direccion = direcciones[5],
                        FechaAlta = DateTime.Now
                    },
                    new Profesional
                    {
                        UserName = "profesional4@ort.edu.ar",
                        Email = "profesional4@ort.edu.ar",
                        NameUser = "profesional4",
                        EmailUser = "profesional4@ort.edu.ar",
                        Nombre = "Dra. Laura",
                        Apellido = "Sánchez",
                        DNI = 20456789,
                        MatriculaId = matriculas[3].Id,
                        PrestacionId = prestaciones[3].Id,
                        HoraInicio = new TimeSpan(10, 0, 0),
                        HoraFin = new TimeSpan(18, 0, 0),
                        Direccion = direcciones[5],
                        FechaAlta = DateTime.Now
                    }
                };

                foreach (var profesional in profesionales)
                {
                    var result = await userManager.CreateAsync(profesional, "Password1!");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(profesional, "Profesional");
                    }
                }
            }
            
          
            // ============= SEED TELÉFONOS =============
            if (!context.Telefonos.Any())
            {
                var paciente1 = context.Pacientes.FirstOrDefault(p => p.UserName == "paciente1@ort.edu.ar");
                var paciente2 = context.Pacientes.FirstOrDefault(p => p.UserName == "paciente2@ort.edu.ar");
                var profesional1 = context.Profesionales.FirstOrDefault(p => p.UserName == "profesional1@ort.edu.ar");
                var profesional2 = context.Profesionales.FirstOrDefault(p => p.UserName == "profesional2@ort.edu.ar");

                if (paciente1 != null && paciente2 != null && profesional1 != null && profesional2 != null)
                {
                    var telefonos = new List<Telefono>
                    {
                        new Telefono
                        {
                            CodigoPais = "+54",
                            Prefijo = "11",
                            Numero = "12345678",
                            TipoDeTelefono = TipoDeTelefono.Movil,
                            PersonaId = paciente1.Id
                        },
                        new Telefono
                        {
                            CodigoPais = "+54",
                            Prefijo = "11",
                            Numero = "23456789",
                            TipoDeTelefono = TipoDeTelefono.Fijo,
                            PersonaId = paciente2.Id
                        },
                        new Telefono
                        {
                            CodigoPais = "+54",
                            Prefijo = "11",
                            Numero = "87654321",
                            TipoDeTelefono = TipoDeTelefono.Movil,
                            PersonaId = profesional1.Id
                        },
                        new Telefono
                        {
                            CodigoPais = "+54",
                            Prefijo = "11",
                            Numero = "98765432",
                            TipoDeTelefono = TipoDeTelefono.Trabajo,
                            PersonaId = profesional2.Id
                        }
                    };
                    context.Telefonos.AddRange(telefonos);
                    context.SaveChanges();
                }
            }

            // ============= SEED TURNOS =============
            // Ahora usamos FechaHora en lugar de Fecha porque el modelo cambió para incluir fecha + hora juntas
            if (!context.Turnos.Any())
            {
                var paciente1 = context.Pacientes.FirstOrDefault(p => p.UserName == "paciente1@ort.edu.ar");
                var paciente2 = context.Pacientes.FirstOrDefault(p => p.UserName == "paciente2@ort.edu.ar");
                var profesional1 = context.Profesionales.FirstOrDefault(p => p.UserName == "profesional1@ort.edu.ar");
                var profesional2 = context.Profesionales.FirstOrDefault(p => p.UserName == "profesional2@ort.edu.ar");

                if (paciente1 != null && paciente2 != null && profesional1 != null && profesional2 != null)
                {
                    var turnos = new List<Turno>
                    {
                        new Turno
                        {
                            FechaHora = DateTime.Now.AddDays(1).AddHours(10),  // Turno mañana a las 10:00
                            FechaDeAlta = DateTime.Now,
                            PacienteId = paciente1.Id,
                            ProfesionalId = profesional1.Id,
                            Confirmado = false,
                            Activo = true
                        },
                        new Turno
                        {
                            FechaHora = DateTime.Now.AddDays(2).AddHours(14),  // Turno pasado mañana a las 14:00
                            FechaDeAlta = DateTime.Now,
                            PacienteId = paciente2.Id,
                            ProfesionalId = profesional2.Id,
                            Confirmado = true,
                            Activo = true
                        },
                        new Turno
                        {
                            FechaHora = DateTime.Now.AddDays(-3).AddHours(9),  // Turno cancelado hace 3 días a las 9:00
                            FechaDeAlta = DateTime.Now.AddDays(-4),
                            PacienteId = paciente1.Id,
                            ProfesionalId = profesional1.Id,
                            Activo = false,
                            DescripcionCancelacion = "Paciente canceló por motivos personales"
                        }
                    };
                    context.Turnos.AddRange(turnos);
                    context.SaveChanges();
                }
            }

            // ============= SEED FORMULARIOS =============
            if (!context.Formularios.Any())
            {
                var coberturas = context.Coberturas.ToList();
                var paciente1 = context.Pacientes.FirstOrDefault(p => p.UserName == "paciente1@ort.edu.ar");

                var formularios = new List<Formulario>
                {
                    new Formulario
                    {
                        Email = "consulta@ejemplo.com",
                        Fecha = DateTime.Now,
                        Nombre = "Pedro",
                        Apellido = "Sánchez",
                        DNI = 11111111,
                        Leido = false,
                        Titulo = "Consulta sobre turnos",
                        Mensaje = "Quisiera saber cómo solicitar un turno online",
                        CoberturaId = coberturas[0].Id,
                        Prestadora = Prestadora.OSDE,
                        PacienteId = null // Anónimo
                    },
                    new Formulario
                    {
                        Email = "info@test.com",
                        Fecha = DateTime.Now.AddDays(-2),
                        Nombre = "Laura",
                        Apellido = "Díaz",
                        DNI = 22222222,
                        Leido = false,
                        Titulo = "Consulta sobre cobertura",
                        Mensaje = "¿Aceptan mi obra social?",
                        CoberturaId = coberturas[2].Id,
                        Prestadora = Prestadora.Galeno,
                        PacienteId = null // Anónimo
                    }
                };
                
                // Agregar formulario del paciente solo si existe
                if (paciente1 != null)
                {
                    formularios.Add(new Formulario
                    {
                        Email = paciente1.Nombre.ToLower() + "@ejemplo.com",
                        Fecha = DateTime.Now.AddDays(-1),
                        Nombre = paciente1.Nombre,
                        Apellido = paciente1.Apellido,
                        DNI = paciente1.DNI,
                        Leido = true,
                        Titulo = "Cambio de turno",
                        Mensaje = "Necesito cambiar mi turno programado",
                        CoberturaId = paciente1.CoberturaId,
                        Prestadora = Prestadora.OSDE,
                        PacienteId = paciente1.Id,
                    });
                }
                
                context.Formularios.AddRange(formularios);
                context.SaveChanges();
            }
        }
    }
}
