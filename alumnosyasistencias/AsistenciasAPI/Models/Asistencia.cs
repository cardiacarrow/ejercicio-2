namespace AsistenciasAPI.Models
{
    public class Asistencia
    {
        public int Id { get; set; }
        public int AlumnoId { get; set; }
        public DateTime Fecha { get; set; }
        public bool Presente { get; set; }
    }
}
