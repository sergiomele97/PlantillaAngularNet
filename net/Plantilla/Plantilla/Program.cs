using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Plantilla.Data.BBDD.Contexto;
using Plantilla.Data.Repositorios;
using Plantilla.Modelos.Entidades;
using Plantilla.Services;

namespace Plantilla
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddUserSecrets<Program>();

            // Configuración del contexto de la base de datos
            builder.Services.AddDbContext<MyDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Agregar Identity
            builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<MyDbContext>()
                .AddDefaultTokenProviders();

            // Configuración del repositorio de usuarios
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            // Registrar el servicio de usuario
            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseMiddleware<ErrorHandlingMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication(); // Middleware de autenticación
            app.UseAuthorization();  // Middleware de autorización

            app.MapControllers();
            app.Run();
        }
    }
}
