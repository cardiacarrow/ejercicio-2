using AsistenciasAPI.Models;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// =============================
// 📌 Inicializar listas en cache
// =============================
void InicializarCache(IMemoryCache cache)
{
    if (!cache.TryGetValue("Alumnos", out List<Alumno> _))
    {
        cache.Set("Alumnos", new List<Alumno>
        {
            new Alumno { Id = 1, Nombre = "Juan Pérez", Matricula = "A123", Grupo = "3A" },
            new Alumno { Id = 2, Nombre = "María López", Matricula = "A124", Grupo = "3A" }
        });
    }

    if (!cache.TryGetValue("Asistencias", out List<Asistencia> _))
    {
        cache.Set("Asistencias", new List<Asistencia>());
    }
}

// =========================
// 📌 Endpoints para Alumnos
// =========================
app.MapGet("/api/alumnos", (IMemoryCache cache) =>
{
    InicializarCache(cache);
    return cache.Get<List<Alumno>>("Alumnos");
});

app.MapGet("/api/alumnos/{id:int}", (int id, IMemoryCache cache) =>
{
    InicializarCache(cache);
    var alumnos = cache.Get<List<Alumno>>("Alumnos")!;
    var alumno = alumnos.FirstOrDefault(a => a.Id == id);
    return alumno is not null ? Results.Ok(alumno) : Results.NotFound();
});

app.MapPost("/api/alumnos", (Alumno nuevo, IMemoryCache cache) =>
{
    InicializarCache(cache);
    var alumnos = cache.Get<List<Alumno>>("Alumnos")!;
    nuevo.Id = alumnos.Any() ? alumnos.Max(a => a.Id) + 1 : 1;
    alumnos.Add(nuevo);
    cache.Set("Alumnos", alumnos);
    return Results.Created($"/api/alumnos/{nuevo.Id}", nuevo);
});

app.MapPut("/api/alumnos/{id:int}", (int id, Alumno actualizado, IMemoryCache cache) =>
{
    InicializarCache(cache);
    var alumnos = cache.Get<List<Alumno>>("Alumnos")!;
    var alumno = alumnos.FirstOrDefault(a => a.Id == id);
    if (alumno is null) return Results.NotFound();

    alumno.Nombre = actualizado.Nombre;
    alumno.Matricula = actualizado.Matricula;
    alumno.Grupo = actualizado.Grupo;

    cache.Set("Alumnos", alumnos);
    return Results.Ok(alumno);
});

app.MapDelete("/api/alumnos/{id:int}", (int id, IMemoryCache cache) =>
{
    InicializarCache(cache);
    var alumnos = cache.Get<List<Alumno>>("Alumnos")!;
    var alumno = alumnos.FirstOrDefault(a => a.Id == id);
    if (alumno is null) return Results.NotFound();

    alumnos.Remove(alumno);
    cache.Set("Alumnos", alumnos);
    return Results.Ok(new { mensaje = "Alumno eliminado correctamente" });
});

// ===========================
// 📌 Endpoints para Asistencias
// ===========================
app.MapGet("/api/asistencias", (IMemoryCache cache) =>
{
    InicializarCache(cache);
    return cache.Get<List<Asistencia>>("Asistencias");
});

app.MapGet("/api/asistencias/fecha/{fecha}", (string fecha, IMemoryCache cache) =>
{
    InicializarCache(cache);
    if (!DateTime.TryParse(fecha, out DateTime fechaBuscada))
        return Results.BadRequest("Formato de fecha inválido. Use yyyy-MM-dd");

    var asistencias = cache.Get<List<Asistencia>>("Asistencias")!
        .Where(a => a.Fecha.Date == fechaBuscada.Date)
        .ToList();

    return Results.Ok(asistencias);
});

app.MapPost("/api/asistencias", (Asistencia nueva, IMemoryCache cache) =>
{
    InicializarCache(cache);
    var asistencias = cache.Get<List<Asistencia>>("Asistencias")!;
    nueva.Id = asistencias.Any() ? asistencias.Max(a => a.Id) + 1 : 1;
    asistencias.Add(nueva);
    cache.Set("Asistencias", asistencias);
    return Results.Created($"/api/asistencias/{nueva.Id}", nueva);
});

app.MapPut("/api/asistencias/{id:int}", (int id, Asistencia actualizada, IMemoryCache cache) =>
{
    InicializarCache(cache);
    var asistencias = cache.Get<List<Asistencia>>("Asistencias")!;
    var asistencia = asistencias.FirstOrDefault(a => a.Id == id);
    if (asistencia is null) return Results.NotFound();

    asistencia.AlumnoId = actualizada.AlumnoId;
    asistencia.Fecha = actualizada.Fecha;
    asistencia.Presente = actualizada.Presente;

    cache.Set("Asistencias", asistencias);
    return Results.Ok(asistencia);
});

app.MapDelete("/api/asistencias/{id:int}", (int id, IMemoryCache cache) =>
{
    InicializarCache(cache);
    var asistencias = cache.Get<List<Asistencia>>("Asistencias")!;
    var asistencia = asistencias.FirstOrDefault(a => a.Id == id);
    if (asistencia is null) return Results.NotFound();

    asistencias.Remove(asistencia);
    cache.Set("Asistencias", asistencias);
    return Results.Ok(new { mensaje = "Asistencia eliminada correctamente" });
});

app.Run();
