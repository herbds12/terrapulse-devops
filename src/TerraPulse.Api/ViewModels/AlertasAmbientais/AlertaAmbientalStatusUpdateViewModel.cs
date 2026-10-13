using System.ComponentModel.DataAnnotations;
using TerraPulse.Api.Models.Enums;

namespace TerraPulse.Api.ViewModels.AlertasAmbientais;

public class AlertaAmbientalStatusUpdateViewModel
{
    [Required(ErrorMessage = "O status do alerta é obrigatório.")]
    public StatusAlerta Status { get; set; }
}