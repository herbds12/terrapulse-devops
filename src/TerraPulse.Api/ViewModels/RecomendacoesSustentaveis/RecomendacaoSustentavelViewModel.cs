using TerraPulse.Api.Models.Enums;

namespace TerraPulse.Api.ViewModels.RecomendacoesSustentaveis;

public class RecomendacaoSustentavelViewModel
{
    public int Id { get; set; }

    public int AlertaAmbientalId { get; set; }

    public string? AlertaTitulo { get; set; }

    public string? AreaMonitoradaNome { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    public string? BeneficioEsperado { get; set; }

    public StatusRecomendacao Status { get; set; }

    public string StatusDescricao { get; set; } = string.Empty;

    public DateTime DataGeracao { get; set; }

    public DateTime? DataAplicacao { get; set; }
}