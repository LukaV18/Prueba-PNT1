using System.ComponentModel.DataAnnotations;

namespace Agenda_C.Models
{
    public enum TipoMatricula
    {
        [Display(Name = "Matrícula Nacional")]
        MN,
        
        [Display(Name = "Matrícula Provincial")]
        MP
    }
}
