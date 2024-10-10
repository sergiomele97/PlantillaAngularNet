using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Plantilla.Data.Entidades
{
    public class User : IdentityUser
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string Email { get; set; }

        [Required]
        [MaxLength(256)]
        public string Password { get; set; }

        [Required]
        [MaxLength(256)]
        public string Username { get; set; }



        // Agrega más propiedades según tus necesidades
    }
}