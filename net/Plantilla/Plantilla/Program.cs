using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Plantilla._2_Servicios;
using Plantilla.Data.BBDD.Contexto;
using Plantilla.Modelos.Entidades;
using Plantilla.Services;
using Serilog;
using System.Text;

namespace Plantilla
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Configura Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                Log.Information("Iniciando la aplicación");

                // BUILDER: Configura los servicios ----------------------------------------------------------------------------
                var builder = WebApplication.CreateBuilder(args);

                // 3.2_DbContext
                builder.Services.AddDbContext<MyDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

                // 3.1_Identity
                builder.Services.AddIdentity<User, IdentityRole>(options =>
                {
                    // Configuración de opciones de Identity para prevenir ataques de fuerza bruta
                    options.Lockout.MaxFailedAccessAttempts = 5; // Máximo de intentos fallidos
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15); // Tiempo de bloqueo
                    options.Lockout.AllowedForNewUsers = true; // Permitir bloqueo para nuevos usuarios
                    options.User.RequireUniqueEmail = true; // Requerir correos electrónicos únicos
                })
                .AddEntityFrameworkStores<MyDbContext>()
                .AddDefaultTokenProviders();

                // 2.1_Servicio usuarios
                builder.Services.AddScoped<IUserService, UserService>();

                // 2.2_Servicio de JWT en método separado
                ConfigureJwtAuthentication(builder);

                // 1_Controladores
                builder.Services.AddControllers();

                // Swagger
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                // APP: Configura el pipeline de middleware ------------------------------------------------------------------
                var app = builder.Build();

                // Middleware de logging de solicitudes y respuestas
                app.UseMiddleware<RequestResponseLoggingMiddleware>();

                // Middleware loging de errores: Tiene que ir el primero
                app.UseMiddleware<ErrorHandlingMiddleware>();

                // Variables de entorno
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                // Redirección Https
                app.UseHttpsRedirection();

                // Autenticación
                app.UseAuthentication();

                // Autorización
                app.UseAuthorization();

                // Configurar cabeceras de seguridad
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

                // Mapear los controladores: Tiene que ir el último
                app.MapControllers();
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "La aplicación no pudo iniciarse");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        // MÉTODOS EXTRAIDOS ------------------------------------------------------------------

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
