using System.ComponentModel.DataAnnotations;

namespace Agenda_C.Models
{
    public enum TipoDeTelefono
    {
        [Display(Name = "Móvil")]
        Movil,
        
        [Display(Name = "Fijo")]
        Fijo,
        
        [Display(Name = "Trabajo")]
        Trabajo
    }
}
