using System;
using System.ComponentModel.DataAnnotations;

namespace Agenda_C.Models
{
    public class Formulario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "El email solo puede contener letras, números y los caracteres . _ % + - @")]
        [EmailAddress(ErrorMessage = "El mail no tiene el formato valido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(50, ErrorMessage = "El campo {0} no debe superar los {1} caracteres")]
        [MinLength(2, ErrorMessage = "El campo {0} debe superar los {1} caracteres")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El campo {0} solo puede contener letras")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(50, ErrorMessage = "El campo {0} no debe superar los {1} caracteres")]
        [MinLength(2, ErrorMessage = "El campo {0} debe superar los {1} caracteres")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El campo {0} solo puede contener letras")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Range(1, 99999999, ErrorMessage = "El campo {0} debe estar entre {1} y {2}")]
        public int DNI { get; set; }

        public bool Leido { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(500, ErrorMessage = "El campo {0} no debe superar los {1} caracteres")]
        [MinLength(10, ErrorMessage = "El campo {0} debe tener al menos {1} caracteres")]
        [DataType(DataType.MultilineText)]
        public string Mensaje { get; set; }

        public Cobertura Cobertura { get; set; }

        public int? CoberturaId { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public Prestadora Prestadora { get; set; }

        public Paciente Paciente { get; set; }

        public String PacienteId { get; set; }
    }
}
