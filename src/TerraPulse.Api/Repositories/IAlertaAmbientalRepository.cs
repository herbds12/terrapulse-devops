using TerraPulse.Api.Models;
using TerraPulse.Api.Models.Enums;

namespace TerraPulse.Api.Repositories;

public interface IAlertaAmbientalRepository
{
    Task<(IEnumerable<AlertaAmbiental> Items, int TotalItems)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        int? areaMonitoradaId = null,
        StatusAlerta? status = null,
        SeveridadeAlerta? severidade = null);

    Task<AlertaAmbiental?> GetByIdAsync(int id);

    Task<AlertaAmbiental> AddAsync(AlertaAmbiental alertaAmbiental);

    Task<IEnumerable<AlertaAmbiental>> AddRangeAsync(IEnumerable<AlertaAmbiental> alertasAmbientais);

    Task<AlertaAmbiental> UpdateAsync(AlertaAmbiental alertaAmbiental);

    Task<bool> ExistsAsync(int id);
}