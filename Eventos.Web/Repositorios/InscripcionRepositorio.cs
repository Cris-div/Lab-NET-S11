using System.Data;
using Eventos.Web.Data;
using Eventos.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
namespace Eventos.Web.Repositorios;

public record ResultadoInscripcion(bool Guardado, string Campo = "", string Error = "");

public class InscripcionRepositorio(EventosContext db)
{
    public async Task<List<Inscripcion>> ListarAsync() =>
        await db.Inscripciones.AsNoTracking().Include(i => i.Evento).Include(i => i.Asistente)
            .OrderByDescending(i => i.FechaInscripcion).ThenByDescending(i => i.Id).ToListAsync();

    public async Task<Inscripcion?> ObtenerAsync(int id) =>
        await db.Inscripciones.AsNoTracking().Include(i => i.Evento).Include(i => i.Asistente)
            .FirstOrDefaultAsync(i => i.Id == id);

    public async Task<List<SelectListItem>> EventosAsync() =>
        await db.Eventos.AsNoTracking().Where(e => e.Activo).OrderBy(e => e.Fecha)
            .Select(e => new SelectListItem(e.Titulo, e.Id.ToString())).ToListAsync();

    public async Task<List<SelectListItem>> AsistentesAsync() =>
        await db.Asistentes.AsNoTracking().Where(a => a.Activo).OrderBy(a => a.Nombres)
            .Select(a => new SelectListItem(a.Nombres + " · " + a.Dni, a.Id.ToString())).ToListAsync();

    public async Task<ResultadoInscripcion> CrearAsync(Inscripcion datos)
    {
        // Validación y guardado en la misma transacción para evitar sobreventa concurrente.
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var evento = await db.Eventos.AsNoTracking().FirstOrDefaultAsync(e => e.Id == datos.EventoId && e.Activo);
        if (evento is null) return new(false, nameof(datos.EventoId), "Selecciona un evento activo.");
        if (evento.Fecha <= DateTime.Now) return new(false, nameof(datos.EventoId), "El evento ya pasó. No se permiten nuevas inscripciones.");
        if (!await db.Asistentes.AsNoTracking().AnyAsync(a => a.Id == datos.AsistenteId && a.Activo))
            return new(false, nameof(datos.AsistenteId), "Selecciona un asistente activo.");
        if (await db.Inscripciones.AsNoTracking().AnyAsync(i => i.EventoId == datos.EventoId && i.AsistenteId == datos.AsistenteId && i.Estado == "Confirmada"))
            return new(false, nameof(datos.AsistenteId), "El asistente ya tiene una inscripción confirmada en este evento.");
        var confirmados = await db.Inscripciones.AsNoTracking().CountAsync(i => i.EventoId == datos.EventoId && i.Estado == "Confirmada");
        if (confirmados >= evento.Aforo) return new(false, nameof(datos.EventoId), "El aforo está completo. No quedan cupos disponibles.");
        db.Inscripciones.Add(new Inscripcion {
            EventoId = datos.EventoId, AsistenteId = datos.AsistenteId,
            Estado = "Confirmada", FechaInscripcion = DateTime.Now
        });
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return new(true);
    }

    public async Task<bool> CancelarAsync(int id)
    {
        var inscripcion = await db.Inscripciones.SingleOrDefaultAsync(i => i.Id == id && i.Estado == "Confirmada");
        if (inscripcion is null) return false;
        inscripcion.Estado = "Cancelada";
        await db.SaveChangesAsync();
        return true;
    }
}
