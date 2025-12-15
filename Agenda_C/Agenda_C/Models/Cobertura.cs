using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Agenda_C.Models
{
    public class Cobertura
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "El campo {0} debe ser un número positivo")]
        public int NumeroCredencial { get; set; }

        public Paciente Paciente { get; set; }

        public int? PacienteId { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public Prestadora Prestadora { get; set; }

        public List<Formulario> Formularios { get; set; }
    }
}
