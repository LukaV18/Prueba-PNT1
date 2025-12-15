using Agenda_C.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Agenda_C.Data
{
    public class AgendaContext : IdentityDbContext<Persona>
    {
        public AgendaContext(DbContextOptions<AgendaContext> options)
            : base(options)
        {
        }

        // DbSets para todas las entidades
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Profesional> Profesionales { get; set; }
        public DbSet<Turno> Turnos { get; set; }
        public DbSet<Prestacion> Prestaciones { get; set; }
        public DbSet<Formulario> Formularios { get; set; }
        public DbSet<Cobertura> Coberturas { get; set; }
        public DbSet<Matricula> Matriculas { get; set; }
        public DbSet<Telefono> Telefonos { get; set; }
        public DbSet<Direccion> Direcciones { get; set; }
        public DbSet<Administrador> Administradores { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar discriminador para herencia TPH (Table Per Hierarchy)
            modelBuilder.Entity<Persona>()
                .HasDiscriminator<string>("PersonaType")
                .HasValue<Paciente>("Paciente")
                .HasValue<Profesional>("Profesional");

            // Relación Turno ↔ Paciente
            modelBuilder.Entity<Paciente>()
                .HasMany(p => p.Turnos)
                .WithOne(t => t.Paciente)
                .HasForeignKey(t => t.PacienteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Turno ↔ Profesional
            modelBuilder.Entity<Profesional>()
                .HasMany(p => p.Turnos)
                .WithOne(t => t.Profesional)
                .HasForeignKey(t => t.ProfesionalId)
                .OnDelete(DeleteBehavior.Restrict);


            // Configuración de relación Paciente-Cobertura (1:1)
            modelBuilder.Entity<Paciente>()
                .HasOne(p => p.Cobertura)
                .WithOne(c => c.Paciente)
                .HasForeignKey<Paciente>(p => p.CoberturaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración de relación Profesional-Matricula (1:1)
            modelBuilder.Entity<Profesional>()
                .HasOne(p => p.Matricula)
                .WithOne(m => m.Profesional)
                .HasForeignKey<Profesional>(p => p.MatriculaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración de relación Profesional-Prestacion (1:N)
            modelBuilder.Entity<Profesional>()
                .HasOne(p => p.Prestacion)
                .WithMany(pr => pr.Profesionales)
                .HasForeignKey(p => p.PrestacionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración de relación Persona-Direccion (1:1)
            modelBuilder.Entity<Direccion>()
                .HasOne(d => d.Persona)
                .WithOne(p => p.Direccion)
                .HasForeignKey<Direccion>(d => d.PersonaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Restricciones de unicidad
            modelBuilder.Entity<Prestacion>()
                .HasIndex(p => p.Nombre)
                .IsUnique();

            modelBuilder.Entity<Cobertura>()
                .HasIndex(c => c.NumeroCredencial)
                .IsUnique();

            modelBuilder.Entity<Matricula>()
                .HasIndex(m => m.Numero)
                .IsUnique();

            // El seed data lo cargamos desde SeedData.cs
        }

        public DbSet<Agenda_C.Models.Persona> Persona { get; set; }
    }
}
