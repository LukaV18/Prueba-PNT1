using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Agenda_C.Models
{
    public abstract class Persona: IdentityUser
    {
        [Required(ErrorMessage = "Debe completar el campo {0}")]
        public int PersonaId { get; set; }

        [Required(ErrorMessage = "Debe completar el campo {0}")]
        [StringLength(15, MinimumLength = 4, ErrorMessage = "El campo {0} solo debe tener un minimo de {2} y un maximo de {1} caracteres.")]
        [RegularExpression(@"^[a-zA-Z.0-9\s]+$", ErrorMessage = "El {0} solo puede contener letras, numeros y puntos.")]
        public string NameUser{ get; set; }

        [Required(ErrorMessage = "Debe completar el campo {0}")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "El email solo puede contener letras, números y los caracteres . _ % + - @")]
        [EmailAddress(ErrorMessage = "El mail no tiene el formato valido")]
        public string EmailUser { get; set; }


        [Required]
        public DateTime FechaAlta { get; set; } = DateTime.Now;


        [Required(ErrorMessage = "Debe completar el campo {0}")]
        [MinLength(2, ErrorMessage = "El campo{0} debe tener un minimo de {1} letras")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚÑñ_\s]+$", ErrorMessage = "El {0} solo puede contener letras.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Debe completar el campo {0}")]
        [MinLength(2, ErrorMessage = "El campo{0} debe tener un minimo de {1} letras")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚÑñ_\s]+$", ErrorMessage = "El {0} solo puede contener letras.")]
        public string Apellido { get; set; }

        public string NombreCompleto => $"{Nombre} {Apellido}";

        [Required(ErrorMessage = "Debe completar el campo {0}")]
        [Range(1, 99999999, ErrorMessage = "El numero debe ser entre {1} y {2}")]
        public int DNI { get; set; }

        [Display(Name = "telefono")]
        public List<Telefono> Telefonos { get; set; }

        public Direccion Direccion { get; set; }

            }
}