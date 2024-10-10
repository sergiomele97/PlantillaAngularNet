using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Plantilla.Data.Entidades;

namespace Plantilla.Data.Contextos
{
    public class MyDbContext : IdentityDbContext<User>
    {
        public MyDbContext(DbContextOptions<DbContext> options)
            : base(options)
        {
        }

        // Puedes agregar otras entidades aquí, si es necesario
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Importante para ASP.NET Identity

            // Puedes agregar restricciones adicionales a los modelos si es necesario
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique(); // El correo electrónico debe ser único
            });
        }
    }
}
