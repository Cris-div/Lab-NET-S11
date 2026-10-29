using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
namespace Eventos.Web.Controllers;

public class AsistentesController(AsistenteRepositorio repositorio) : Controller
{
    public async Task<IActionResult> Index() => View(await repositorio.ListarAsync());
    public async Task<IActionResult> Create()
    {
        await Task.CompletedTask;
        return View(new Asistente());
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nombres,Dni,Correo,Telefono")] Asistente asistente)
    {
        asistente.Nombres = asistente.Nombres?.Trim() ?? "";
        asistente.Dni = asistente.Dni?.Trim() ?? "";
        asistente.Correo = asistente.Correo?.Trim().ToLowerInvariant() ?? "";
        asistente.Telefono = string.IsNullOrWhiteSpace(asistente.Telefono) ? null : asistente.Telefono.Trim();
        if (await repositorio.ExisteDniAsync(asistente.Dni))
            ModelState.AddModelError(nameof(asistente.Dni), "Este DNI ya está registrado.");
        if (await repositorio.ExisteCorreoAsync(asistente.Correo))
            ModelState.AddModelError(nameof(asistente.Correo), "Este correo ya está registrado.");
        if (ModelState.IsValid)
        {
            try
            {
                await repositorio.CrearAsync(asistente);
                TempData["Exito"] = "Asistente registrado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
            {
                ModelState.AddModelError("", "El DNI o correo acaba de ser registrado. Revisa los datos.");
            }
        }
        return View(asistente);
    }
}
