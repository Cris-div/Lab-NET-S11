using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Eventos.Web.Models;

public class Evento
{
    [Key] public int Id { get; set; }
    [Required(ErrorMessage = "Ingresa el título."), StringLength(100), Display(Name = "Título")]
    public string Titulo { get; set; } = "";
    [Required, Display(Name = "Fecha y hora")]
    public DateTime Fecha { get; set; } = DateTime.Today.AddDays(1).AddHours(18);
    [Required(ErrorMessage = "Ingresa el lugar."), StringLength(100)] public string Lugar { get; set; } = "";
    [Range(1, 5000, ErrorMessage = "El aforo debe estar entre 1 y 5000.")] public int Aforo { get; set; } = 30;
    [Column(TypeName = "decimal(10,2)"), Range(typeof(decimal), "0", "99999999.99", ErrorMessage = "Ingresa un precio no negativo.")]
    public decimal Precio { get; set; }
    [Required, StringLength(20), RegularExpression("^(Presencial|Virtual)$", ErrorMessage = "Selecciona Presencial o Virtual.")]
    public string Modalidad { get; set; } = "Presencial";
    [Range(1, int.MaxValue, ErrorMessage = "Selecciona una categoría."), Display(Name = "Categoría")]
    public int CategoriaId { get; set; }
    [ForeignKey(nameof(CategoriaId))] public Categoria? Categoria { get; set; }
    public bool Activo { get; set; } = true;
    public ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();
}
