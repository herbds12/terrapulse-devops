using System.ComponentModel.DataAnnotations;
using TerraPulse.Api.Models.Enums;

namespace TerraPulse.Api.ViewModels.AreasMonitoradas;

public class AreaMonitoradaUpdateViewModel
{
    [Required(ErrorMessage = "O nome da área monitorada é obrigatório.")]
    [MaxLength(120, ErrorMessage = "O nome deve ter no máximo 120 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O país é obrigatório.")]
    [MaxLength(80, ErrorMessage = "O país deve ter no máximo 80 caracteres.")]
    public string Pais { get; set; } = string.Empty;

    [Required(ErrorMessage = "A região é obrigatória.")]
    [MaxLength(120, ErrorMessage = "A região deve ter no máximo 120 caracteres.")]
    public string Regiao { get; set; } = string.Empty;

    [MaxLength(100, ErrorMessage = "A cultura deve ter no máximo 100 caracteres.")]
    public string? Cultura { get; set; }

    [Required(ErrorMessage = "O tipo da área é obrigatório.")]
    public TipoArea TipoArea { get; set; }

    [Required(ErrorMessage = "O status da área é obrigatório.")]
    public StatusArea Status { get; set; }

    [Range(0.01, 9999999, ErrorMessage = "O tamanho em hectares deve ser maior que zero.")]
    public decimal TamanhoHectares { get; set; }

    [Range(-90, 90, ErrorMessage = "A latitude deve estar entre -90 e 90.")]
    public decimal Latitude { get; set; }

    [Range(-180, 180, ErrorMessage = "A longitude deve estar entre -180 e 180.")]
    public decimal Longitude { get; set; }
}