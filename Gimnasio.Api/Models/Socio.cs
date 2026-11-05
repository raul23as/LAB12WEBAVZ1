using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Gimnasio.Api.Models;

public class Socio
{
    public int Id { get; set; }

    [Required, StringLength(80, MinimumLength = 2)]
    public string Nombre { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(120)]
    public string Correo { get; set; } = string.Empty;

    [Required, StringLength(20, MinimumLength = 7)]
    public string Telefono { get; set; } = string.Empty;

    [Range(14, 100)]
    public int Edad { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public ICollection<Membresia> Membresias { get; set; } = new List<Membresia>();
}
