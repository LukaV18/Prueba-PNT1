using System;
using System.ComponentModel.DataAnnotations;

namespace Agenda_C.Models
{
    public class Turno
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public DateTime FechaHora { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public DateTime FechaDeAlta { get; set; }

        // Relación con Usuario como Paciente
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string PacienteId { get; set; }
        public Paciente Paciente { get; set; }

        // Relación con Usuario como Profesional
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string ProfesionalId { get; set; }
        public Profesional Profesional { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public bool Confirmado { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public bool Activo { get; set; }

        [MaxLength(500, ErrorMessage = "El campo {0} no debe superar los {1} caracteres")]
        [DataType(DataType.MultilineText)]
        public string DescripcionCancelacion { get; set; }
    }
}
