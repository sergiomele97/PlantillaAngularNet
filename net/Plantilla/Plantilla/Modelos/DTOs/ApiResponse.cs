using System.Collections.Generic;

namespace Plantilla.Modelos.DTOs
{
    public class ApiResponse<T> // T es un parámetro genérico
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public IEnumerable<string> Errors { get; set; }

        public ApiResponse(bool success, string message, T data, IEnumerable<string> errors = null)
        {
            Success = success;
            Message = message;
            Data = data;
            Errors = errors ?? new List<string>();
        }
    }
}
