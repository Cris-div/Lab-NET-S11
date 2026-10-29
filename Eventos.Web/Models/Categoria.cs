using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
namespace Eventos.Web.Models;

[Index(nameof(Nombre), IsUnique = true)]
public class Categoria
{
    [Key] public int Id { get; set; }
    [Required, StringLength(50)] public string Nombre { get; set; } = "";
    public bool Activo { get; set; } = true;
    public ICollection<Evento> Eventos { get; set; } = new List<Evento>();
}
