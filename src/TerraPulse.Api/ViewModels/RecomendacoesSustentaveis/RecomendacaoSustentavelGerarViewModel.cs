using System.ComponentModel.DataAnnotations;

namespace TerraPulse.Api.ViewModels.RecomendacoesSustentaveis;

public class RecomendacaoSustentavelGerarViewModel
{
    [Required(ErrorMessage = "O ID do alerta ambiental é obrigatório.")]
    [Range(1, int.MaxValue, ErrorMessage = "O ID do alerta ambiental deve ser maior que zero.")]
    public int AlertaAmbientalId { get; set; }
}