namespace AsistenciasAPI.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string ContraseñaHash { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Rol { get; set; }

        // Relación con Docente
        public int DocenteId { get; set; }
        public Docente? Docente { get; set; }
    }
}
