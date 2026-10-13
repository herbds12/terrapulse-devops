using System.ComponentModel.DataAnnotations;
using TerraPulse.Api.Models.Enums;

namespace TerraPulse.Api.Models;

public class AlertaAmbiental
{
    public int Id { get; set; }

    public int AreaMonitoradaId { get; set; }

    public AreaMonitorada? AreaMonitorada { get; set; }

    public TipoAlerta TipoAlerta { get; set; }

    public SeveridadeAlerta Severidade { get; set; }

    public StatusAlerta Status { get; set; } = StatusAlerta.Aberto;

    [Required]
    [MaxLength(160)]
    public string Titulo { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string Descricao { get; set; } = string.Empty;

    public decimal NivelConfianca { get; set; }

    public DateTime DataGeracao { get; set; } = DateTime.UtcNow;

    public DateTime? DataResolucao { get; set; }

    public ICollection<RecomendacaoSustentavel> Recomendacoes { get; set; } = new List<RecomendacaoSustentavel>();
}