using Microsoft.AspNetCore.Mvc;
using AsistenciasAPI.Models;
using Microsoft.Extensions.Caching.Memory;

namespace AsistenciasAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlumnosController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private const string CacheKey = "Alumnos";

        public AlumnosController(IMemoryCache cache)
        {
            _cache = cache;
            InicializarCache();
        }

        private void InicializarCache()
        {
            if (!_cache.TryGetValue(CacheKey, out List<Alumno>? _))
            {
                // 🔹 Inicializamos con lista vacía
                _cache.Set(CacheKey, new List<Alumno>());
            }
        }

        [HttpGet]
        public IActionResult GetAlumnos()
        {
            var alumnos = _cache.Get<List<Alumno>>(CacheKey)!;
            return Ok(alumnos);
        }

        [HttpGet("{id}")]
        public IActionResult GetAlumno(int id)
        {
            var alumnos = _cache.Get<List<Alumno>>(CacheKey)!;
            var alumno = alumnos.FirstOrDefault(a => a.Id == id);
            if (alumno == null) return NotFound(new { mensaje = "Alumno no encontrado" });
            return Ok(alumno);
        }

        [HttpPost]
        public IActionResult CrearAlumno([FromBody] Alumno nuevo)
        {
            var alumnos = _cache.Get<List<Alumno>>(CacheKey)!;

            // Generar ID incremental
            nuevo.Id = alumnos.Any() ? alumnos.Max(a => a.Id) + 1 : 1;

            alumnos.Add(nuevo);
            _cache.Set(CacheKey, alumnos);

            return CreatedAtAction(nameof(GetAlumno), new { id = nuevo.Id }, nuevo);
        }

        [HttpPut("{id}")]
        public IActionResult ActualizarAlumno(int id, [FromBody] Alumno actualizado)
        {
            var alumnos = _cache.Get<List<Alumno>>(CacheKey)!;
            var alumno = alumnos.FirstOrDefault(a => a.Id == id);
            if (alumno == null) return NotFound(new { mensaje = "Alumno no encontrado" });

            alumno.Nombre = actualizado.Nombre;
            alumno.Matricula = actualizado.Matricula;
            alumno.Grupo = actualizado.Grupo;

            _cache.Set(CacheKey, alumnos);
            return Ok(alumno);
        }

        [HttpDelete("{id}")]
        public IActionResult EliminarAlumno(int id)
        {
            var alumnos = _cache.Get<List<Alumno>>(CacheKey)!;
            var alumno = alumnos.FirstOrDefault(a => a.Id == id);
            if (alumno == null) return NotFound(new { mensaje = "Alumno no encontrado" });

            alumnos.Remove(alumno);
            _cache.Set(CacheKey, alumnos);

            return Ok(new { mensaje = "Alumno eliminado correctamente" });
        }
    }
}


