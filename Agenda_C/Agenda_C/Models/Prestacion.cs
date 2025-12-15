using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Agenda_C.Models
{
    public class Prestacion
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(100, ErrorMessage = "El campo {0} no debe superar los {1} caracteres")]
        [MinLength(2, ErrorMessage = "El campo {0} debe tener al menos {1} caracteres")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ0-9\s]+$", ErrorMessage = "El campo {0} solo puede contener letras, números y espacios")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(500, ErrorMessage = "El campo {0} no debe superar los {1} caracteres")]
        [MinLength(10, ErrorMessage = "El campo {0} debe tener al menos {1} caracteres")]
        [DataType(DataType.MultilineText)]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Range(15, 180, ErrorMessage = "El campo {0} debe estar entre {1} y {2} minutos")]
        public int Duracion { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Range(0.01, 999999.99, ErrorMessage = "El campo {0} debe estar entre {1} y {2}")]
        public decimal Precio { get; set; }

        public List<Profesional> Profesionales { get; set; }
    }
}