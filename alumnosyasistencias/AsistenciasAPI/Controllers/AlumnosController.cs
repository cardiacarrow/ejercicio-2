using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AsistenciasAPI.Models;
using AsistenciasAPI.Data;

namespace AsistenciasAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlumnosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AlumnosController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAlumnos()
        {
            var alumnos = await _context.Alumnos.ToListAsync();
            return Ok(alumnos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAlumno(int id)
        {
            var alumno = await _context.Alumnos.FindAsync(id);
            if (alumno == null) return NotFound(new { mensaje = "Alumno no encontrado" });
            return Ok(alumno);
        }

        [HttpPost]
        public async Task<IActionResult> CrearAlumno([FromBody] Alumno nuevo)
        {
            _context.Alumnos.Add(nuevo);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAlumno), new { id = nuevo.Id }, nuevo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarAlumno(int id, [FromBody] Alumno actualizado)
        {
            var alumno = await _context.Alumnos.FindAsync(id);
            if (alumno == null) return NotFound(new { mensaje = "Alumno no encontrado" });

            alumno.Nombre = actualizado.Nombre;
            alumno.Matricula = actualizado.Matricula;
            alumno.Grupo = actualizado.Grupo;

            await _context.SaveChangesAsync();
            return Ok(alumno);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarAlumno(int id)
        {
            var alumno = await _context.Alumnos.FindAsync(id);
            if (alumno == null) return NotFound(new { mensaje = "Alumno no encontrado" });

            _context.Alumnos.Remove(alumno);
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Alumno eliminado correctamente" });
        }

        [HttpGet("test-error")]
        public IActionResult GetError()
        {
            throw new Exception("Prueba de error en el middleware");
        }
    }
}


