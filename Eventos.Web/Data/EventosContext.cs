using Eventos.Web.Models;
using Microsoft.EntityFrameworkCore;
namespace Eventos.Web.Data;

public class EventosContext(DbContextOptions<EventosContext> options) : DbContext(options)
{
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Evento> Eventos => Set<Evento>();
    public DbSet<Asistente> Asistentes => Set<Asistente>();
    public DbSet<Inscripcion> Inscripciones => Set<Inscripcion>();
}
