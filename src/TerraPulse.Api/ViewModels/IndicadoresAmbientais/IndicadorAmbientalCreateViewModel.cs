using System.ComponentModel.DataAnnotations;
using TerraPulse.Api.Models.Enums;

namespace TerraPulse.Api.ViewModels.IndicadoresAmbientais;

public class IndicadorAmbientalCreateViewModel
{
    [Required(ErrorMessage = "O ID da área monitorada é obrigatório.")]
    [Range(1, int.MaxValue, ErrorMessage = "O ID da área monitorada deve ser maior que zero.")]
    public int AreaMonitoradaId { get; set; }

    [Required(ErrorMessage = "O tipo do indicador é obrigatório.")]
    public TipoIndicador TipoIndicador { get; set; }

    [Range(0, 9999999, ErrorMessage = "O valor do indicador deve ser maior ou igual a zero.")]
    public decimal Valor { get; set; }

    [Required(ErrorMessage = "A unidade de medida é obrigatória.")]
    [MaxLength(30, ErrorMessage = "A unidade de medida deve ter no máximo 30 caracteres.")]
    public string UnidadeMedida { get; set; } = string.Empty;

    [MaxLength(150, ErrorMessage = "A fonte deve ter no máximo 150 caracteres.")]
    public string? Fonte { get; set; }

    [MaxLength(500, ErrorMessage = "A observação deve ter no máximo 500 caracteres.")]
    public string? Observacao { get; set; }
}