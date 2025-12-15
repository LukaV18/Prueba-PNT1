using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Agenda_C.Models
{
        public class Profesional : Usuario // mejor heredar de Usuario concreto
        {
            // Relación con Matricula
            public Matricula Matricula { get; set; }
            public int? MatriculaId { get; set; }

            // Relación con Prestacion
            public Prestacion Prestacion { get; set; }
            public int? PrestacionId { get; set; }

            // Relación con Turnos (como Profesional)
            public ICollection<Turno> Turnos { get; set; } = new List<Turno>();

            [Required(ErrorMessage = "El campo {0} es obligatorio")]
            [Display(Name = "Hora de inicio")]
            public TimeSpan HoraInicio { get; set; }   // más seguro que TimeOnly para EF Core

            [Required(ErrorMessage = "El campo {0} es obligatorio")]
            [Display(Name = "Hora de fin")]
            public TimeSpan HoraFin { get; set; }
        }
    }

