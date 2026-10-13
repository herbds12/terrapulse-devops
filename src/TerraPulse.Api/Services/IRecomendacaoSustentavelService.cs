using TerraPulse.Api.Models.Enums;
using TerraPulse.Api.ViewModels.Common;
using TerraPulse.Api.ViewModels.RecomendacoesSustentaveis;

namespace TerraPulse.Api.Services;

public interface IRecomendacaoSustentavelService
{
    Task<PagedResultViewModel<RecomendacaoSustentavelViewModel>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        int? alertaAmbientalId = null,
        StatusRecomendacao? status = null);

    Task<RecomendacaoSustentavelViewModel?> GetByIdAsync(int id);

    Task<RecomendacaoSustentavelViewModel?> GerarRecomendacaoAsync(
        RecomendacaoSustentavelGerarViewModel viewModel);

    Task<RecomendacaoSustentavelViewModel?> AtualizarStatusAsync(
        int id,
        RecomendacaoSustentavelStatusUpdateViewModel viewModel);
}