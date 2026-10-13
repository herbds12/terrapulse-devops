using System.ComponentModel.DataAnnotations;
using TerraPulse.Api.Models.Enums;

namespace TerraPulse.Api.Models;

public class RecomendacaoSustentavel
{
    public int Id { get; set; }

    public int AlertaAmbientalId { get; set; }

    public AlertaAmbiental? AlertaAmbiental { get; set; }

    [Required]
    [MaxLength(160)]
    public string Titulo { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string Descricao { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? BeneficioEsperado { get; set; }

    public StatusRecomendacao Status { get; set; } = StatusRecomendacao.Pendente;

    public DateTime DataGeracao { get; set; } = DateTime.UtcNow;

    public DateTime? DataAplicacao { get; set; }
}