using Microsoft.AspNetCore.Mvc;
using AsistenciasAPI.Models;
using Microsoft.Extensions.Caching.Memory;

namespace AsistenciasAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AsistenciasController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private const string CacheKey = "Asistencias";

        public AsistenciasController(IMemoryCache cache)
        {
            _cache = cache;
            InicializarCache();
        }

        private void InicializarCache()
        {
            if (!_cache.TryGetValue(CacheKey, out List<Asistencia>? _))
            {
                // 🔹 Inicializamos con lista vacía
                _cache.Set(CacheKey, new List<Asistencia>());
            }
        }

        [HttpGet]
        public IActionResult GetAsistencias()
        {
            var asistencias = _cache.Get<List<Asistencia>>(CacheKey)!;
            return Ok(asistencias);
        }

        [HttpGet("fecha/{fecha}")]
        public IActionResult GetAsistenciasPorFecha(string fecha)
        {
            if (!DateTime.TryParse(fecha, out DateTime fechaBuscada))
                return BadRequest(new { mensaje = "Formato de fecha inválido. Use yyyy-MM-dd" });

            var asistencias = _cache.Get<List<Asistencia>>(CacheKey)!
                .Where(a => a.Fecha.Date == fechaBuscada.Date)
                .ToList();

            return Ok(asistencias);
        }

        [HttpPost]
        public IActionResult RegistrarAsistencia([FromBody] Asistencia nueva)
        {
            var asistencias = _cache.Get<List<Asistencia>>(CacheKey)!;

            // ✅ Evitar duplicados: mismo AlumnoId + misma Fecha
            if (asistencias.Any(a => a.AlumnoId == nueva.AlumnoId && a.Fecha.Date == nueva.Fecha.Date))
                return BadRequest(new { mensaje = "El alumno ya tiene asistencia registrada para esta fecha." });

            nueva.Id = asistencias.Any() ? asistencias.Max(a => a.Id) + 1 : 1;
            asistencias.Add(nueva);

            _cache.Set(CacheKey, asistencias);
            return CreatedAtAction(nameof(GetAsistencias), new { id = nueva.Id }, nueva);
        }

        [HttpPut("{id}")]
        public IActionResult ActualizarAsistencia(int id, [FromBody] Asistencia actualizada)
        {
            var asistencias = _cache.Get<List<Asistencia>>(CacheKey)!;
            var asistencia = asistencias.FirstOrDefault(a => a.Id == id);
            if (asistencia == null) return NotFound(new { mensaje = "Asistencia no encontrada" });

            // ✅ Validar duplicado al actualizar
            if (asistencias.Any(a => a.Id != id && a.AlumnoId == actualizada.AlumnoId && a.Fecha.Date == actualizada.Fecha.Date))
                return BadRequest(new { mensaje = "Ya existe una asistencia para este alumno en esa fecha." });

            asistencia.AlumnoId = actualizada.AlumnoId;
            asistencia.Fecha = actualizada.Fecha;
            asistencia.Presente = actualizada.Presente;

            _cache.Set(CacheKey, asistencias);
            return Ok(asistencia);
        }

        [HttpDelete("{id}")]
        public IActionResult EliminarAsistencia(int id)
        {
            var asistencias = _cache.Get<List<Asistencia>>(CacheKey)!;
            var asistencia = asistencias.FirstOrDefault(a => a.Id == id);
            if (asistencia == null) return NotFound(new { mensaje = "Asistencia no encontrada" });

            asistencias.Remove(asistencia);
            _cache.Set(CacheKey, asistencias);

            return Ok(new { mensaje = "Asistencia eliminada correctamente" });
        }
    }
}
