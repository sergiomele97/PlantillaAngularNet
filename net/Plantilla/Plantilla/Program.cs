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

                // Configura DbContext
                builder.Services.AddDbContext<MyDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

                // Configura Identity
                builder.Services.AddIdentity<User, IdentityRole>(options =>
                {
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                    options.Lockout.AllowedForNewUsers = true;
                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<MyDbContext>()
                .AddDefaultTokenProviders();

                // Configura servicios
                builder.Services.AddScoped<IUserService, UserService>();

                // Configura JWT
                ConfigureJwtAuthentication(builder);

                // Configura CORS
                ConfigureCors(builder);

                // Controladores
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

                // Middleware de CORS
                app.UseCors("AllowDevelopment");

                // Autenticación
                app.UseAuthentication();

                // Autorización
                app.UseAuthorization();

                // Configurar cabeceras de seguridad
                app.Use(async (context, next) =>
                {
                    context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'");
                    context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
                    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
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
                var expirationInMinutes = 60;
                return new TokenService(secret, expirationInMinutes);
            });
        }

        private static void ConfigureCors(WebApplicationBuilder builder)
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowDevelopment", builder =>
                {
                    builder.WithOrigins("http://localhost:4200")
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials();
                });
            });
        }

    }
}
