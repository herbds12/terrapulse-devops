using TerraPulse.Api.Models.Enums;

namespace TerraPulse.Api.ViewModels.AreasMonitoradas;

public class AreaMonitoradaViewModel
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Pais { get; set; } = string.Empty;

    public string Regiao { get; set; } = string.Empty;

    public string? Cultura { get; set; }

    public TipoArea TipoArea { get; set; }

    public string TipoAreaDescricao { get; set; } = string.Empty;

    public StatusArea Status { get; set; }

    public string StatusDescricao { get; set; } = string.Empty;

    public decimal TamanhoHectares { get; set; }

    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }

    public DateTime DataCadastro { get; set; }

    public DateTime? UltimaSincronizacao { get; set; }
}