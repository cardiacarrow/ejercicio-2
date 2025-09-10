using AsistenciasAPI.Models;
using Microsoft.Extensions.Caching.Memory;
using AsistenciasAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);


// Agregar servicios
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services.AddControllers(); // 👈 Importante para usar tus Controllers

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionMiddleware();
app.UseHttpsRedirection();
app.MapControllers();
app.UseHttpsRedirection();

app.MapControllers(); // 👈 Habilita los controladores de la carpeta Controllers

// =============================
// 📌 Inicializar listas en cache (vacías)
// =============================
using (var scope = app.Services.CreateScope())
{
    var cache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();

    if (!cache.TryGetValue("Alumnos", out List<Alumno>? _))
    {
        cache.Set("Alumnos", new List<Alumno>()); // lista vacía
    }

    if (!cache.TryGetValue("Asistencias", out List<Asistencia>? _))
    {
        cache.Set("Asistencias", new List<Asistencia>()); // lista vacía
    }
}

app.Run();