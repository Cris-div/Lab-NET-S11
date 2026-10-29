using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Eventos.Web.Models;

public class Inscripcion
{
    [Key] public int Id { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Selecciona un evento."), Display(Name = "Evento")] public int EventoId { get; set; }
    [ForeignKey(nameof(EventoId))] public Evento? Evento { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Selecciona un asistente."), Display(Name = "Asistente")] public int AsistenteId { get; set; }
    [ForeignKey(nameof(AsistenteId))] public Asistente? Asistente { get; set; }
    [Display(Name = "Fecha de inscripción")] public DateTime FechaInscripcion { get; set; } = DateTime.Now;
    [Required, StringLength(10), RegularExpression("^(Confirmada|Cancelada)$")] public string Estado { get; set; } = "Confirmada";
}
