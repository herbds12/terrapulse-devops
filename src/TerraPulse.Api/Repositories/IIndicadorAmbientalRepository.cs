using TerraPulse.Api.Models;
using TerraPulse.Api.Models.Enums;

namespace TerraPulse.Api.Repositories;

public interface IIndicadorAmbientalRepository
{
    Task<(IEnumerable<IndicadorAmbiental> Items, int TotalItems)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        int? areaMonitoradaId = null,
        TipoIndicador? tipoIndicador = null);

    Task<IndicadorAmbiental?> GetByIdAsync(int id);

    Task<IndicadorAmbiental> AddAsync(IndicadorAmbiental indicadorAmbiental);

    Task<bool> DeleteAsync(int id);

    Task<bool> ExistsAsync(int id);
}