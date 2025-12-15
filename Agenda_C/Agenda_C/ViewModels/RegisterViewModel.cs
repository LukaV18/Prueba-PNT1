using System.ComponentModel.DataAnnotations;

namespace Agenda_C.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(100)]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El DNI es obligatorio")]
        [Range(1000000, 99999999, ErrorMessage = "El DNI debe tener entre 7 y 8 dígitos")]
        [Display(Name = "DNI")]
        public int DNI { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del email es inválido")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(100, MinimumLength = 8,
            ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Debe confirmar la contraseña")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        [Display(Name = "Confirmar contraseña")]
        public string ConfirmPassword { get; set; }
    }
}
    

