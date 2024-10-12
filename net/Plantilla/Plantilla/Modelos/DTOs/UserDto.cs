using System.ComponentModel.DataAnnotations;

namespace Plantilla.Modelos.DTOs
{
    public class UserDto
    {
        [Required(ErrorMessage = "Email es obligatorio.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        [StringLength(100)]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(100, ErrorMessage = "La contraseña debe tener al menos {2} caracteres.", MinimumLength = 6)]
        public string Password { get; set; }
    }
}
