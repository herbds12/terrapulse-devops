using TerraPulse.Api.Models.Enums;
using TerraPulse.Api.ViewModels.AreasMonitoradas;
using TerraPulse.Api.ViewModels.Common;

namespace TerraPulse.Api.Services;

public interface IAreaMonitoradaService
{
    Task<PagedResultViewModel<AreaMonitoradaViewModel>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        StatusArea? status = null,
        TipoArea? tipoArea = null);

    Task<AreaMonitoradaViewModel?> GetByIdAsync(int id);

    Task<AreaMonitoradaViewModel> CreateAsync(AreaMonitoradaCreateViewModel viewModel);

    Task<AreaMonitoradaViewModel?> UpdateAsync(int id, AreaMonitoradaUpdateViewModel viewModel);

    Task<bool> DeleteAsync(int id);
}