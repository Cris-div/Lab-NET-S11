using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
namespace Eventos.Web.Models;

[Index(nameof(Dni), IsUnique = true)]
[Index(nameof(Correo), IsUnique = true)]
public class Asistente
{
    [Key] public int Id { get; set; }
    [Required(ErrorMessage = "Ingresa los nombres."), StringLength(100)] public string Nombres { get; set; } = "";
    [Required(ErrorMessage = "Ingresa el DNI."), StringLength(8), RegularExpression(@"^[0-9]{8}$", ErrorMessage = "El DNI debe tener exactamente 8 dígitos."), Display(Name = "DNI")]
    public string Dni { get; set; } = "";
    [Required(ErrorMessage = "Ingresa el correo."), EmailAddress(ErrorMessage = "Ingresa un correo válido."), StringLength(254)]
    public string Correo { get; set; } = "";
    [StringLength(9, ErrorMessage = "El teléfono admite hasta 9 caracteres."), Display(Name = "Teléfono")]
    public string? Telefono { get; set; }
    public bool Activo { get; set; } = true;
    public ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();
}
