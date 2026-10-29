using Eventos.Web.Data;
using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<EventoRepositorio>();
builder.Services.AddScoped<AsistenteRepositorio>();
builder.Services.AddScoped<InscripcionRepositorio>();
builder.Services.Configure<RequestLocalizationOptions>(options =>
    options.SetDefaultCulture("es-PE").AddSupportedCultures("es-PE").AddSupportedUICultures("es-PE"));
builder.Services.AddDbContext<EventosContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EventosContext")));

var app = builder.Build();

// Los datos se cargan solamente después de aplicar las migraciones con dotnet ef.
// --seed-only permite cargar el estado inicial antes de la segunda migración.
if (!app.Environment.IsEnvironment("DesignTime"))
{
    await using var scope = app.Services.CreateAsyncScope();
    var db = scope.ServiceProvider.GetRequiredService<EventosContext>();
    if (!await db.Categorias.AnyAsync())
    {
        db.Categorias.AddRange(new Categoria { Nombre = "Tecnología" },
            new Categoria { Nombre = "Emprendimiento" }, new Categoria { Nombre = "Diseño" });
        await db.SaveChangesAsync();
    }
    if (!await db.Eventos.AnyAsync())
    {
        var categorias = await db.Categorias.AsNoTracking().OrderBy(c => c.Id).ToListAsync();
        var fecha = DateTime.Today.AddDays(14).AddHours(18);
        db.Eventos.AddRange(
            new Evento { Titulo = "Desarrollo web con .NET", Fecha = fecha, Lugar = "Auditorio principal", Aforo = 60, Precio = 35, CategoriaId = categorias[0].Id },
            new Evento { Titulo = "Ideas que se convierten en negocio", Fecha = fecha.AddDays(2), Lugar = "Sala de innovación", Aforo = 40, Precio = 20, CategoriaId = categorias[1].Id },
            new Evento { Titulo = "Diseño de experiencias digitales", Fecha = fecha.AddDays(5), Lugar = "Laboratorio creativo", Aforo = 25, Precio = 45, CategoriaId = categorias[2].Id },
            new Evento { Titulo = "Introducción a la inteligencia artificial", Fecha = fecha.AddDays(7), Lugar = "Aula virtual", Aforo = 100, Precio = 0, CategoriaId = categorias[0].Id });
        await db.SaveChangesAsync();
    }
    if (!await db.Asistentes.AnyAsync())
    {
        db.Asistentes.AddRange(
            new Asistente { Nombres = "Ana Torres", Dni = "70000001", Correo = "ana.torres@example.com" },
            new Asistente { Nombres = "Bruno Medina", Dni = "70000002", Correo = "bruno.medina@example.com" },
            new Asistente { Nombres = "Carla Rojas", Dni = "70000003", Correo = "carla.rojas@example.com" },
            new Asistente { Nombres = "Diego Salas", Dni = "70000004", Correo = "diego.salas@example.com" },
            new Asistente { Nombres = "Elena Vargas", Dni = "70000005", Correo = "elena.vargas@example.com" });
        await db.SaveChangesAsync();
    }
}
if (args.Contains("--seed-only")) return;

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseRequestLocalization();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Eventos}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
