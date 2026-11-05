using System.ComponentModel.DataAnnotations;

namespace Gimnasio.Api.Models;

public class Membresia
{
    public int Id { get; set; }

    [Required, StringLength(20, MinimumLength = 4)]
    public string Codigo { get; set; } = string.Empty;

    [Required, StringLength(20)]
    public string TipoPlan { get; set; } = string.Empty;

    [Range(typeof(decimal), "10", "1000")]
    public decimal PrecioMensual { get; set; }

    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }

    [Range(1, int.MaxValue)]
    public int SocioId { get; set; }

    public Socio? Socio { get; set; }
}
