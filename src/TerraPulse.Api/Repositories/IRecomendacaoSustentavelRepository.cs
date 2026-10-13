using TerraPulse.Api.Models;
using TerraPulse.Api.Models.Enums;

namespace TerraPulse.Api.Repositories;

public interface IRecomendacaoSustentavelRepository
{
    Task<(IEnumerable<RecomendacaoSustentavel> Items, int TotalItems)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        int? alertaAmbientalId = null,
        StatusRecomendacao? status = null);

    Task<RecomendacaoSustentavel?> GetByIdAsync(int id);

    Task<RecomendacaoSustentavel> AddAsync(RecomendacaoSustentavel recomendacaoSustentavel);

    Task<RecomendacaoSustentavel> UpdateAsync(RecomendacaoSustentavel recomendacaoSustentavel);

    Task<bool> ExistsAsync(int id);
}