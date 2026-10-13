using TerraPulse.Api.Models.Enums;
using TerraPulse.Api.ViewModels.AlertasAmbientais;
using TerraPulse.Api.ViewModels.Common;

namespace TerraPulse.Api.Services;

public interface IAlertaAmbientalService
{
    Task<PagedResultViewModel<AlertaAmbientalViewModel>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        int? areaMonitoradaId = null,
        StatusAlerta? status = null,
        SeveridadeAlerta? severidade = null);

    Task<AlertaAmbientalViewModel?> GetByIdAsync(int id);

    Task<IEnumerable<AlertaAmbientalViewModel>?> GerarAlertasAsync(
        AlertaAmbientalGerarViewModel viewModel);

    Task<AlertaAmbientalViewModel?> AtualizarStatusAsync(
        int id,
        AlertaAmbientalStatusUpdateViewModel viewModel);
}