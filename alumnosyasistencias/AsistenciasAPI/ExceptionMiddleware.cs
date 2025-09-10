using System.Net;
using System.Text.Json;

namespace AsistenciasAPI.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Pasar al siguiente middleware/controlador
                await _next(context);
            }
            catch (Exception ex)
            {
                // Loguear la excepción
                _logger.LogError(ex, "Ocurrió un error inesperado en la aplicación");

                // Preparar la respuesta JSON
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var respuesta = new
                {
                    mensaje = "Ha ocurrido un error interno en el servidor.",
                    detalle = ex.Message, // ⚠️ en producción normalmente se omite el detalle
                    statusCode = context.Response.StatusCode
                };

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(respuesta, options);

                await context.Response.WriteAsync(json);
            }
        }
    }
}
