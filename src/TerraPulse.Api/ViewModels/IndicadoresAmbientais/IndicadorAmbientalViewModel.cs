using TerraPulse.Api.Models.Enums;

namespace TerraPulse.Api.ViewModels.IndicadoresAmbientais;

public class IndicadorAmbientalViewModel
{
    public int Id { get; set; }

    public int AreaMonitoradaId { get; set; }

    public string? AreaMonitoradaNome { get; set; }

    public TipoIndicador TipoIndicador { get; set; }

    public string TipoIndicadorDescricao { get; set; } = string.Empty;

    public decimal Valor { get; set; }

    public string UnidadeMedida { get; set; } = string.Empty;

    public DateTime DataLeitura { get; set; }

    public string? Fonte { get; set; }

    public string? Observacao { get; set; }
}