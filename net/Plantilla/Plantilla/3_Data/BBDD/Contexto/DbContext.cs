using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Plantilla.Modelos.Entidades;

namespace Plantilla.Data.BBDD.Contexto
{
    public class MyDbContext : IdentityDbContext<User>
    {
        public MyDbContext(DbContextOptions<MyDbContext> options)
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
                entity.HasIndex(e => e.Email).IsUnique();
            });
        }

        // DbSet para tus entidades
        public DbSet<User> Users { get; set; }
    }
}
