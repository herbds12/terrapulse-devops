using TerraPulse.Api.Models.Enums;
using TerraPulse.Api.ViewModels.Common;
using TerraPulse.Api.ViewModels.IndicadoresAmbientais;

namespace TerraPulse.Api.Services;

public interface IIndicadorAmbientalService
{
    Task<PagedResultViewModel<IndicadorAmbientalViewModel>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        int? areaMonitoradaId = null,
        TipoIndicador? tipoIndicador = null);

    Task<IndicadorAmbientalViewModel?> GetByIdAsync(int id);

    Task<IndicadorAmbientalViewModel?> CreateAsync(IndicadorAmbientalCreateViewModel viewModel);

    Task<bool> DeleteAsync(int id);
}