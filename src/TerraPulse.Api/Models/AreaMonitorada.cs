using System.ComponentModel.DataAnnotations;
using TerraPulse.Api.Models.Enums;

namespace TerraPulse.Api.Models;

public class AreaMonitorada
{
    public int Id { get; set; }

    [Required]
    [MaxLength(120)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [MaxLength(80)]
    public string Pais { get; set; } = string.Empty;

    [Required]
    [MaxLength(120)]
    public string Regiao { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Cultura { get; set; }

    public TipoArea TipoArea { get; set; }

    public StatusArea Status { get; set; } = StatusArea.Ativa;

    public decimal TamanhoHectares { get; set; }

    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }

    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

    public DateTime? UltimaSincronizacao { get; set; }

    public ICollection<IndicadorAmbiental> Indicadores { get; set; } = new List<IndicadorAmbiental>();

    public ICollection<AlertaAmbiental> Alertas { get; set; } = new List<AlertaAmbiental>();
}