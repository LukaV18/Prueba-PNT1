using System.ComponentModel.DataAnnotations;

namespace Agenda_C.Models
{
    public enum Prestadora
    {
        [Display(Name = "OSDE")]
        OSDE,
        
        [Display(Name = "Galeno")]
        Galeno,
        
        [Display(Name = "Swiss Medical")]
        SwissMedical,
        
        [Display(Name = "Sancor Salud")]
        SancorSalud,
        
        [Display(Name = "Medicus")]
        Medicus,
        
        [Display(Name = "Omint")]
        Omint,
        
        [Display(Name = "Unión Personal")]
        UnionPersona
    }
}
