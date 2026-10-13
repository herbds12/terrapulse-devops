using System.ComponentModel.DataAnnotations;

namespace TerraPulse.Api.ViewModels.AlertasAmbientais;

public class AlertaAmbientalGerarViewModel
{
    [Required(ErrorMessage = "O ID da área monitorada é obrigatório.")]
    [Range(1, int.MaxValue, ErrorMessage = "O ID da área monitorada deve ser maior que zero.")]
    public int AreaMonitoradaId { get; set; }
}