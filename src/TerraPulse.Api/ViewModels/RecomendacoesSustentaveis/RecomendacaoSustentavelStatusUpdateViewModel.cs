using System.ComponentModel.DataAnnotations;
using TerraPulse.Api.Models.Enums;

namespace TerraPulse.Api.ViewModels.RecomendacoesSustentaveis;

public class RecomendacaoSustentavelStatusUpdateViewModel
{
    [Required(ErrorMessage = "O status da recomendação é obrigatório.")]
    public StatusRecomendacao Status { get; set; }
}