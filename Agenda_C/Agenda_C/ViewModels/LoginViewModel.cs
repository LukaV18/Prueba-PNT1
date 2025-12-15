using System.ComponentModel.DataAnnotations;

namespace Agenda_C.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del email es inválido")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Display(Name = "Recordarme")]
        public bool RememberMe { get; set; }
    }
}
    

