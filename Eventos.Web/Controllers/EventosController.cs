using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Eventos.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
namespace Eventos.Web.Controllers;

public class EventosController(EventoRepositorio repositorio) : Controller
{
    public async Task<IActionResult> Index(int? categoriaId) => View(new EventosIndexViewModel {
        CategoriaId = categoriaId, Eventos = await repositorio.ListarAsync(categoriaId),
        Categorias = await repositorio.CategoriasAsync()
    });

    public async Task<IActionResult> Details(int id)
    {
        var evento = await repositorio.ObtenerAsync(id);
        return evento is null ? NotFound() : View(evento);
    }
    public async Task<IActionResult> Create()
    {
        ViewBag.Categorias = await repositorio.CategoriasAsync();
        return View(new Evento());
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Titulo,Fecha,Lugar,Aforo,Precio,CategoriaId,Modalidad")] Evento evento)
    {
        if (!await repositorio.CategoriaActivaAsync(evento.CategoriaId))
            ModelState.AddModelError(nameof(evento.CategoriaId), "Selecciona una categoría activa.");
        if (ModelState.IsValid)
        {
            evento.Titulo = evento.Titulo.Trim();
            evento.Lugar = evento.Lugar.Trim();
            await repositorio.CrearAsync(evento);
            TempData["Exito"] = "Evento creado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Categorias = await repositorio.CategoriasAsync();
        return View(evento);
    }
    public async Task<IActionResult> Edit(int id)
    {
        var fila = await repositorio.ObtenerAsync(id);
        if (fila is null) return NotFound();
        ViewBag.Categorias = await repositorio.CategoriasAsync();
        return View(fila.Evento);
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Fecha,Lugar,Aforo,Precio,CategoriaId,Modalidad")] Evento evento)
    {
        if (id != evento.Id) return BadRequest();
        if (await repositorio.ObtenerAsync(id) is null) return NotFound();
        if (!await repositorio.CategoriaActivaAsync(evento.CategoriaId))
            ModelState.AddModelError(nameof(evento.CategoriaId), "Selecciona una categoría activa.");
        if (ModelState.IsValid)
        {
            var error = await repositorio.ActualizarAsync(evento);
            if (error is null)
            {
                TempData["Exito"] = "Evento actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError(nameof(evento.Aforo), error);
        }
        ViewBag.Categorias = await repositorio.CategoriasAsync();
        return View(evento);
    }
    public async Task<IActionResult> Delete(int id)
    {
        var evento = await repositorio.ObtenerAsync(id);
        return evento is null ? NotFound() : View(evento);
    }
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var evento = await repositorio.ObtenerAsync(id);
        if (evento is null) return NotFound();
        if (!ModelState.IsValid) return View("Delete", evento);
        if (!await repositorio.EliminarAsync(id)) return NotFound();
        TempData["Exito"] = "Evento desactivado. Se conservó su historial de inscripciones.";
        return RedirectToAction(nameof(Index));
    }
}
