using System.ComponentModel.DataAnnotations;

namespace Agenda_C.Models
{
    public class Direccion
    {
        public int Id { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9 \s]+$", ErrorMessage = "El {0} solo puede contener letras.")]
        [Required(ErrorMessage = "Debe completar el campo{0}")]
        public string Calle { get; set; }

        [Required(ErrorMessage = "Debe completar el campo{0}")]
        public int Altura { get; set; }

        public int? Piso { get; set; }

        [MaxLength(10, ErrorMessage = "El campo {0} no debe superar los {1} caracteres")]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "El {0} solo puede contener letras o números.")]
        public string Departamento { get; set; }

        public Persona Persona { get; set; }

        public string PersonaId { get; set; }
    }
}
