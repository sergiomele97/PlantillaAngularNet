using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Plantilla._2_Servicios;
using Plantilla.Data.BBDD.Contexto;
using Plantilla.Data.Repositorios;
using Plantilla.Modelos.Entidades;
using Plantilla.Services;
using System.Text;

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

            //      2.1_Servicio usuarios
            builder.Services.AddScoped<IUserService, UserService>();

            //      2.2_Servicio de JWT en método separado
            ConfigureJwtAuthentication(builder);

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









        //  MÉTODOS EXTRAIDOS ------------------------------------------------------------------

        // Configurar JWT
        private static void ConfigureJwtAuthentication(WebApplicationBuilder builder)
        {
            var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtConfig:Secret"]);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.AddScoped<TokenService>(provider =>
            {
                var secret = builder.Configuration["JwtConfig:Secret"];
                var expirationInMinutes = 60; // Establece el tiempo de expiración como desees
                return new TokenService(secret, expirationInMinutes);
            });
        }
    }
}
