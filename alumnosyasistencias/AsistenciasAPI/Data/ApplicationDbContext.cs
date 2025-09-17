using AsistenciasAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AsistenciasAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Tablas (DbSets)
        public DbSet<Alumno> Alumnos { get; set; }
        public DbSet<Asistencia> Asistencias { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Docente> Docentes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Alumno
            modelBuilder.Entity<Alumno>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(a => a.Matricula).IsRequired().HasMaxLength(20);
                entity.Property(a => a.Grupo).IsRequired().HasMaxLength(50);
            });

            // Asistencia
            modelBuilder.Entity<Asistencia>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Fecha).IsRequired();
                entity.Property(a => a.Presente).IsRequired();
            });

            // Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.NombreUsuario).IsRequired().HasMaxLength(50);
                entity.Property(u => u.ContraseñaHash).IsRequired();
                entity.Property(u => u.NombreCompleto).IsRequired().HasMaxLength(150);

                entity.HasIndex(u => u.NombreUsuario).IsUnique(); // único
            });

            // Docente
            modelBuilder.Entity<Docente>(entity =>
            {
                entity.HasKey(d => d.Id);
                entity.Property(d => d.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(d => d.Apellido).IsRequired().HasMaxLength(100);
                entity.Property(d => d.Email).IsRequired().HasMaxLength(100);
            });

            // Relación Docente → Usuarios
            modelBuilder.Entity<Docente>()
                .HasMany(d => d.Usuarios)
                .WithOne(u => u.Docente)
                .HasForeignKey(u => u.DocenteId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
