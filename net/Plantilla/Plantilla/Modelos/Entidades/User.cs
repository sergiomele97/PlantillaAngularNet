using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Plantilla.Modelos.Entidades
{
   
    public class User : IdentityUser // No necesitas redefinir propiedades existentes
    {
        // Puedes agregar más propiedades personalizadas
        // [MaxLength(256)]
        //public string CustomField { get; set; } // Ejemplo de campo personalizado

    }
}
