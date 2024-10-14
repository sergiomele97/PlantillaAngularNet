using Microsoft.AspNetCore.Http;
using Serilog;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestResponseLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Registra la solicitud
        var request = context.Request;

        // Habilita el buffering para poder leer el cuerpo de la solicitud varias veces
        request.EnableBuffering();

        // Lee el cuerpo de la solicitud
        string bodyAsText;
        using (var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true)) // leaveOpen para no cerrar el stream
        {
            bodyAsText = await reader.ReadToEndAsync(); // Lee el cuerpo de la solicitud
            request.Body.Position = 0; // Resetea la posición para que otros middleware puedan leerlo
        }

        // Procesa el cuerpo de la solicitud para ocultar la contraseña
        bodyAsText = MaskPassword(bodyAsText);

        // Log de la solicitud
        Log.Information("Solicitud: {Method} {Path} {Body}", request.Method, request.Path, bodyAsText);

        // Temporizador para medir el tiempo de procesamiento
        var stopwatch = Stopwatch.StartNew();

        // Llama al siguiente middleware en la cadena
        await _next(context);

        // Registra la respuesta
        stopwatch.Stop();
        var response = context.Response;

        Log.Information("Respuesta: {StatusCode} {ElapsedMilliseconds} ms", response.StatusCode, stopwatch.ElapsedMilliseconds);
    }

    private string MaskPassword(string body)
    {
        // Intenta convertir el cuerpo a un objeto JSON
        try
        {
            var json = JObject.Parse(body);
            if (json["password"] != null)
            {
                json["password"] = "*****"; // Reemplaza el valor de la contraseña por un marcador
            }
            return json.ToString();
        }
        catch
        {
            // Si no se puede analizar como JSON, devuelve el cuerpo original
            return body;
        }
    }
}
