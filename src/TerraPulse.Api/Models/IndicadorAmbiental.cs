using System.ComponentModel.DataAnnotations;
using TerraPulse.Api.Models.Enums;

namespace TerraPulse.Api.Models;

public class IndicadorAmbiental
{
    public int Id { get; set; }

    public int AreaMonitoradaId { get; set; }

    public AreaMonitorada? AreaMonitorada { get; set; }

    public TipoIndicador TipoIndicador { get; set; }

    public decimal Valor { get; set; }

    [MaxLength(30)]
    public string UnidadeMedida { get; set; } = string.Empty;

    public DateTime DataLeitura { get; set; } = DateTime.UtcNow;

    [MaxLength(150)]
    public string? Fonte { get; set; }

    [MaxLength(500)]
    public string? Observacao { get; set; }
}