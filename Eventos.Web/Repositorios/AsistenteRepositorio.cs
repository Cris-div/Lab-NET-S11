using Eventos.Web.Data;
using Eventos.Web.Models;
using Microsoft.EntityFrameworkCore;
namespace Eventos.Web.Repositorios;

public class AsistenteRepositorio(EventosContext db)
{
    public async Task<List<Asistente>> ListarAsync() =>
        await db.Asistentes.AsNoTracking().Where(a => a.Activo).OrderBy(a => a.Nombres).ToListAsync();
    // También se comprueban registros inactivos, porque el índice único cubre toda la tabla.
    public async Task<bool> ExisteDniAsync(string dni) =>
        await db.Asistentes.AsNoTracking().AnyAsync(a => a.Dni == dni);
    public async Task<bool> ExisteCorreoAsync(string correo) =>
        await db.Asistentes.AsNoTracking().AnyAsync(a => a.Correo == correo);
    public async Task CrearAsync(Asistente asistente)
    {
        asistente.Id = 0;
        asistente.Activo = true;
        db.Asistentes.Add(asistente);
        await db.SaveChangesAsync();
    }
}
