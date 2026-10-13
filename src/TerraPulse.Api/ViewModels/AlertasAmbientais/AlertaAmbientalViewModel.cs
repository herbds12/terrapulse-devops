using TerraPulse.Api.Models.Enums;

namespace TerraPulse.Api.ViewModels.AlertasAmbientais;

public class AlertaAmbientalViewModel
{
    public int Id { get; set; }

    public int AreaMonitoradaId { get; set; }

    public string? AreaMonitoradaNome { get; set; }

    public TipoAlerta TipoAlerta { get; set; }

    public string TipoAlertaDescricao { get; set; } = string.Empty;

    public SeveridadeAlerta Severidade { get; set; }

    public string SeveridadeDescricao { get; set; } = string.Empty;

    public StatusAlerta Status { get; set; }

    public string StatusDescricao { get; set; } = string.Empty;

    public string Titulo { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    public decimal NivelConfianca { get; set; }

    public DateTime DataGeracao { get; set; }

    public DateTime? DataResolucao { get; set; }
}