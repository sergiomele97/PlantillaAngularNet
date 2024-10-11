using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        // Por defecto, se asume que es un error interno
        var code = HttpStatusCode.InternalServerError;
        var message = "Ocurrió un error inesperado.";

        // Manejo de excepciones específicas
        switch (ex)
        {
            case ArgumentNullException argNullEx:
                code = HttpStatusCode.BadRequest; // Bad Request
                message = argNullEx.Message; // Mensaje específico de ArgumentNullException
                break;

            case ArgumentException argEx:
                code = HttpStatusCode.BadRequest; // Bad Request
                message = argEx.Message; // Mensaje específico de ArgumentException
                break;

            case InvalidOperationException invOpEx:
                code = HttpStatusCode.BadRequest; // Bad Request
                message = invOpEx.Message; // Mensaje específico de InvalidOperationException
                break;

            case DbUpdateException dbEx:
                code = HttpStatusCode.BadRequest; // Bad Request
                message = "Ocurrió un error al intentar crear o actualizar el usuario.";
                break;

            case KeyNotFoundException keyNotFoundEx:
                code = HttpStatusCode.NotFound; // Not Found
                message = keyNotFoundEx.Message; // Mensaje específico de KeyNotFoundException
                break;

            case UnauthorizedAccessException _:
                code = HttpStatusCode.Forbidden; // Forbidden
                message = "No tienes permisos para acceder a este recurso.";
                break;

            // Otros casos adicionales
            case FormatException formatEx:
                code = HttpStatusCode.BadRequest; // Bad Request
                message = "Formato de datos inválido."; // Mensaje específico para errores de formato
                break;

            case TimeoutException timeoutEx:
                code = HttpStatusCode.RequestTimeout; // Request Timeout
                message = "La solicitud ha superado el tiempo de espera."; // Mensaje para tiempo de espera
                break;

            // Puedes agregar más excepciones según lo necesites

            default:
                // Manejo de otras excepciones no específicas
                break;
        }

        // Configurar la respuesta
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        // Crear el objeto de respuesta
        var response = new
        {
            statusCode = context.Response.StatusCode,
            message
        };

        // Serializar y devolver la respuesta
        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
