using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Plantilla.Modelos.DTOs;
using System.Net;
using System.Text.Json;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next; 
        _logger = logger; 
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); // Llamar al siguiente middleware
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex); // Manejar la excepción
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var code = HttpStatusCode.InternalServerError; // Código de estado por defecto
        var message = "Ocurrió un error inesperado."; // Mensaje de error por defecto

        // Manejo de excepciones específicas
        switch (ex)
        {
            case ArgumentNullException argNullEx:
                code = HttpStatusCode.BadRequest;
                message = "argNullEx.Message"; 
                break;

            case ArgumentException argEx:
                code = HttpStatusCode.BadRequest;
                message = argEx.Message; 
                break;

            case InvalidOperationException invOpEx:
                code = HttpStatusCode.BadRequest;
                message = invOpEx.Message; 
                break;

            case DbUpdateException dbEx:
                code = HttpStatusCode.BadRequest;
                message = "Ocurrió un error al intentar crear o actualizar el usuario."; 
                break;

            case KeyNotFoundException keyNotFoundEx:
                code = HttpStatusCode.NotFound;
                message = keyNotFoundEx.Message; 
                break;

            case UnauthorizedAccessException _:
                code = HttpStatusCode.Forbidden;
                message = "No tienes permisos para acceder a este recurso."; 
                break;

            case FormatException formatEx:
                code = HttpStatusCode.BadRequest;
                message = "Formato de datos inválido."; 
                break;

            case TimeoutException timeoutEx:
                code = HttpStatusCode.RequestTimeout;
                message = "La solicitud ha superado el tiempo de espera."; 
                break;

            default:
                message = "Ocurrió un error inesperado."; 
                break;
        }

        // Registrar el error
        _logger.LogError(ex, message); // Aquí se registra el error

        context.Response.ContentType = "application/json"; 
        context.Response.StatusCode = (int)code; 

        // Crear la respuesta estandarizada
        var response = new ApiResponse<object>(false, message, null, new List<string> { message });

        return context.Response.WriteAsync(JsonSerializer.Serialize(response)); // Enviar la respuesta
    }
}
