using System.ComponentModel.DataAnnotations;

namespace Agenda_C.Models
{
    public class Matricula
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "El campo {0} debe ser un número positivo")]
        public int Numero { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public Provincia Provincia { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public TipoMatricula Tipo { get; set; }

        public Profesional Profesional { get; set; }

        public int? ProfesionalId { get; set; }
    }
}
