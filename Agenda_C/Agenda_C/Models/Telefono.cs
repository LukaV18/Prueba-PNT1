using System.ComponentModel.DataAnnotations;

namespace Agenda_C.Models
{
    public class Telefono
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [RegularExpression(@"^\+?\d{1,4}$", ErrorMessage = "El {0} debe tener entre 1 y 4 dígitos (puede empezar con +)")]
        [Display(Name = "Código de País")]
        public string CodigoPais { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [RegularExpression(@"^\d{2,5}$", ErrorMessage = "El {0} debe contener entre 2 y 5 dígitos")]
        [Display(Name = "Código de Área/Prefijo")]
        public string Prefijo { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [RegularExpression(@"^\d{6,10}$", ErrorMessage = "El {0} debe contener entre 6 y 10 dígitos")]
        [StringLength(10, MinimumLength = 6, ErrorMessage = "El {0} debe tener entre {2} y {1} dígitos")]
        [Display(Name = "Número")]
        public string Numero { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Tipo de Teléfono")]
        public TipoDeTelefono TipoDeTelefono { get; set; }

        // Relación con Usuario (antes Persona abstracta)
        public string PersonaId { get; set; }
        public Usuario Persona { get; set; }

        [Display(Name = "Teléfono Completo")]
        public string TelefonoCompleto => $"{CodigoPais} {Prefijo} {Numero}";
    }
}