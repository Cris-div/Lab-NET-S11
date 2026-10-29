using System.Data;
using Eventos.Web.Data;
using Eventos.Web.Models;
using Eventos.Web.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
namespace Eventos.Web.Repositorios;

public class EventoRepositorio(EventosContext db)
{
    public async Task<List<EventoFila>> ListarAsync(int? categoriaId) =>
        await db.Eventos.AsNoTracking().Include(e => e.Categoria)
            .Where(e => e.Activo && (!categoriaId.HasValue || e.CategoriaId == categoriaId))
            .OrderBy(e => e.Fecha).ThenBy(e => e.Titulo)
            .Select(e => new EventoFila { Evento = e, Confirmados = e.Inscripciones.Count(i => i.Estado == "Confirmada") })
            .ToListAsync();

    public async Task<EventoFila?> ObtenerAsync(int id) =>
        await db.Eventos.AsNoTracking().Include(e => e.Categoria).Where(e => e.Id == id && e.Activo)
            .Select(e => new EventoFila { Evento = e, Confirmados = e.Inscripciones.Count(i => i.Estado == "Confirmada") })
            .FirstOrDefaultAsync();

    public async Task<List<SelectListItem>> CategoriasAsync() =>
        await db.Categorias.AsNoTracking().Where(c => c.Activo).OrderBy(c => c.Nombre)
            .Select(c => new SelectListItem(c.Nombre, c.Id.ToString())).ToListAsync();

    public async Task<bool> CategoriaActivaAsync(int id) =>
        await db.Categorias.AsNoTracking().AnyAsync(c => c.Id == id && c.Activo);

    public async Task CrearAsync(Evento evento)
    {
        evento.Id = 0;
        evento.Activo = true;
        db.Eventos.Add(evento);
        await db.SaveChangesAsync();
    }

    // Se serializa con las inscripciones para que una reducción del aforo no exceda los cupos.
    public async Task<string?> ActualizarAsync(Evento datos)
    {
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var evento = await db.Eventos.SingleOrDefaultAsync(e => e.Id == datos.Id && e.Activo);
        if (evento is null) return "El evento ya no está disponible.";
        var confirmados = await db.Inscripciones.AsNoTracking().CountAsync(i => i.EventoId == datos.Id && i.Estado == "Confirmada");
        if (datos.Aforo < confirmados) return $"El aforo no puede ser menor a los {confirmados} inscritos confirmados.";
        evento.Titulo = datos.Titulo.Trim();
        evento.Fecha = datos.Fecha;
        evento.Lugar = datos.Lugar.Trim();
        evento.Aforo = datos.Aforo;
        evento.Precio = datos.Precio;
        evento.CategoriaId = datos.CategoriaId;
        evento.Modalidad = datos.Modalidad;
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return null;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var evento = await db.Eventos.SingleOrDefaultAsync(e => e.Id == id && e.Activo);
        if (evento is null) return false;
        evento.Activo = false;
        await db.SaveChangesAsync();
        return true;
    }
}
