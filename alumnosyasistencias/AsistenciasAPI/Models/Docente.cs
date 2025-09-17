using System.Collections.Generic;

namespace AsistenciasAPI.Models
{
    public class Docente
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Relación con Usuarios
        public ICollection<Usuario>? Usuarios { get; set; }
    }
}
