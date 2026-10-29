using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
namespace Eventos.Web.Controllers;

public class InscripcionesController(InscripcionRepositorio repositorio) : Controller
{
    public async Task<IActionResult> Index() => View(await repositorio.ListarAsync());
    public async Task<IActionResult> Create(int? eventoId)
    {
        await CargarListasAsync();
        return View(new Inscripcion { EventoId = eventoId ?? 0 });
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("EventoId,AsistenteId")] Inscripcion inscripcion)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var resultado = await repositorio.CrearAsync(inscripcion);
                if (resultado.Guardado)
                {
                    TempData["Exito"] = "Inscripción confirmada correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(resultado.Campo, resultado.Error);
            }
            catch (SqlException ex) when (ex.Number == 1205)
            {
                ModelState.AddModelError("", "Otro registro se procesó al mismo tiempo. Intenta nuevamente.");
            }
        }
        await CargarListasAsync();
        return View(inscripcion);
    }
    public async Task<IActionResult> Cancelar(int id)
    {
        var inscripcion = await repositorio.ObtenerAsync(id);
        return inscripcion is null ? NotFound() : View(inscripcion);
    }
    [HttpPost, ActionName("Cancelar"), ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelarConfirmado(int id)
    {
        var inscripcion = await repositorio.ObtenerAsync(id);
        if (inscripcion is null) return NotFound();
        if (!ModelState.IsValid) return View("Cancelar", inscripcion);
        var cancelada = await repositorio.CancelarAsync(id);
        TempData[cancelada ? "Exito" : "Aviso"] = cancelada ? "Inscripción cancelada. El cupo vuelve a estar disponible." : "La inscripción ya estaba cancelada.";
        return RedirectToAction(nameof(Index));
    }
    private async Task CargarListasAsync()
    {
        ViewBag.Eventos = await repositorio.EventosAsync();
        ViewBag.Asistentes = await repositorio.AsistentesAsync();
    }
}
