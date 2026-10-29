using Eventos.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace Eventos.Web.ViewModels;

public class EventoFila
{
    public Evento Evento { get; set; } = new();
    public int Confirmados { get; set; }
    public int Disponibles => Math.Max(0, Evento.Aforo - Confirmados);
}
public class EventosIndexViewModel
{
    public List<EventoFila> Eventos { get; set; } = [];
    public List<SelectListItem> Categorias { get; set; } = [];
    public int? CategoriaId { get; set; }
}
