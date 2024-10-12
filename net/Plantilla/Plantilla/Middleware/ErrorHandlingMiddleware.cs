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
        var code = HttpStatusCode.InternalServerError;
        var message = "Ocurrió un error inesperado.";

        // Manejo de excepciones específicas
        switch (ex)
        {
            case ArgumentNullException argNullEx:
                code = HttpStatusCode.BadRequest;
                message = argNullEx.Message;
                break;

            case ArgumentException argEx:
                code = HttpStatusCode.BadRequest;
                message = argEx.Message;
                break;

            case InvalidOperationException invOpEx:
                // Detectamos excepciones relacionadas con operaciones inválidas, como errores de creación de usuarios
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
                // Si no es una excepción específica, devolvemos un error genérico 500
                message = "Ocurrió un error inesperado.";
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        var response = new
        {
            statusCode = context.Response.StatusCode,
            message
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

}
