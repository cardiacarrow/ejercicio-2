using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AsistenciasAPI.Models;
using AsistenciasAPI.Data;
using Microsoft.AspNetCore.Authorization;

namespace AsistenciasAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // 🔒 Todos los endpoints requieren autenticación
    public class AsistenciasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AsistenciasController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsistencias()
        {
            var asistencias = await _context.Asistencias.ToListAsync();
            return Ok(asistencias);
        }

        [HttpGet("fecha/{fecha}")]
        public async Task<IActionResult> GetAsistenciasPorFecha(string fecha)
        {
            if (!DateTime.TryParse(fecha, out DateTime fechaBuscada))
                return BadRequest(new { mensaje = "Formato de fecha inválido. Use yyyy-MM-dd" });

            var asistencias = await _context.Asistencias
                .Where(a => a.Fecha.Date == fechaBuscada.Date)
                .ToListAsync();

            return Ok(asistencias);
        }

        [HttpPost]
        [Authorize(Roles = "Docente")] // 🔒 Solo Docente puede registrar asistencias
        public async Task<IActionResult> RegistrarAsistencia([FromBody] Asistencia nueva)
        {
            bool existe = await _context.Asistencias
                .AnyAsync(a => a.AlumnoId == nueva.AlumnoId && a.Fecha.Date == nueva.Fecha.Date);

            if (existe)
                return BadRequest(new { mensaje = "El alumno ya tiene asistencia registrada para esta fecha." });

            _context.Asistencias.Add(nueva);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAsistencias), new { id = nueva.Id }, nueva);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Docente")] // 🔒 Solo Docente puede actualizar asistencias
        public async Task<IActionResult> ActualizarAsistencia(int id, [FromBody] Asistencia actualizada)
        {
            var asistencia = await _context.Asistencias.FindAsync(id);
            if (asistencia == null) return NotFound(new { mensaje = "Asistencia no encontrada" });

            bool duplicado = await _context.Asistencias
                .AnyAsync(a => a.Id != id && a.AlumnoId == actualizada.AlumnoId && a.Fecha.Date == actualizada.Fecha.Date);

            if (duplicado)
                return BadRequest(new { mensaje = "Ya existe una asistencia para este alumno en esa fecha." });

            asistencia.AlumnoId = actualizada.AlumnoId;
            asistencia.Fecha = actualizada.Fecha;
            asistencia.Presente = actualizada.Presente;

            await _context.SaveChangesAsync();
            return Ok(asistencia);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Docente")] // 🔒 Solo Docente puede eliminar asistencias
        public async Task<IActionResult> EliminarAsistencia(int id)
        {
            var asistencia = await _context.Asistencias.FindAsync(id);
            if (asistencia == null) return NotFound(new { mensaje = "Asistencia no encontrada" });

            _context.Asistencias.Remove(asistencia);
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Asistencia eliminada correctamente" });
        }
    }
}