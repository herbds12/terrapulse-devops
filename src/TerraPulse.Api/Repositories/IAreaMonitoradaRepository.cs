using TerraPulse.Api.Models;
using TerraPulse.Api.Models.Enums;

namespace TerraPulse.Api.Repositories;

public interface IAreaMonitoradaRepository
{
    Task<(IEnumerable<AreaMonitorada> Items, int TotalItems)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        StatusArea? status = null,
        TipoArea? tipoArea = null);

    Task<AreaMonitorada?> GetByIdAsync(int id);

    Task<AreaMonitorada> AddAsync(AreaMonitorada areaMonitorada);

    Task<AreaMonitorada> UpdateAsync(AreaMonitorada areaMonitorada);

    Task<bool> DeleteAsync(int id);

    Task<bool> ExistsAsync(int id);
}