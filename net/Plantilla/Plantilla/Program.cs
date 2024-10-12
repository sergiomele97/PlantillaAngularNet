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
            //  BUILDER: Configura los servicios ----------------------------------------------------------------------------
            var builder = WebApplication.CreateBuilder(args);

            //      3.3_DbContext
            builder.Services.AddDbContext<MyDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            //      3.2_Identity
            builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<MyDbContext>()
                .AddDefaultTokenProviders();

            //      3.1_Repositorio usuarios
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            //      2_Servicio usuarios
            builder.Services.AddScoped<IUserService, UserService>();

            //      1_Controladores
            builder.Services.AddControllers();

            //      Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //  APP: Configura el pipeline de middleware ------------------------------------------------------------------
            var app = builder.Build();

            //      Middleware errores: Tiene que ir el primero
            app.UseMiddleware<ErrorHandlingMiddleware>();

            //      Variables de entorno
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //      Redirección Https
            app.UseHttpsRedirection();

            //      Autenticación
            app.UseAuthentication();

            //      Autorización
            app.UseAuthorization();

            //      Configurar cabeceras de seguridad
            app.Use(async (context, next) =>
            {
                // Content Security Policy
                context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'");

                // Strict Transport Security
                context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");

                // X-Content-Type-Options
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");

                // X-Frame-Options
                context.Response.Headers.Add("X-Frame-Options", "DENY");

                await next();
            });

            //      Mapear los controladores: Tiene que ir el último
            app.MapControllers();
            app.Run();
        }
    }
}
