
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Agenda_C.Models
{
    // Clase concreta que hereda de Persona (abstracta)
    public class Usuario : Persona
    {
        // Tipo de usuario dentro del sistema (Paciente, Profesional, Admin, etc.)
        [Required(ErrorMessage = "Debe especificar el tipo de usuario")]
        [Display(Name = "Tipo de Usuario")]
        public string TipoUsuario { get; set; }

        // Estado de la cuenta
        public bool Activo { get; set; } = true;

        // Fecha de último acceso
        public DateTime? UltimoAcceso { get; set; }

        // Rol principal (además de los roles de Identity)
        [StringLength(50)]
        public string RolPrincipal { get; set; }

        // Datos adicionales
        [Display(Name = "Fecha de nacimiento")]
        public DateTime? FechaNacimiento { get; set; }

        [Display(Name = "Género")]
        public string Genero { get; set; }
        // Relación con Turnos (como Paciente)
        public ICollection<Turno> TurnosPaciente { get; set; } = new List<Turno>();

        // Relación con Turnos (como Profesional)
        public ICollection<Turno> TurnosProfesional { get; set; } = new List<Turno>();
    }
}

