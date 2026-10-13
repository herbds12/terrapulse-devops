using AutoMapper;
using TerraPulse.Api.Models;
using TerraPulse.Api.Models.Enums;
using TerraPulse.Api.Repositories;
using TerraPulse.Api.ViewModels.Common;
using TerraPulse.Api.ViewModels.IndicadoresAmbientais;

namespace TerraPulse.Api.Services;

public class IndicadorAmbientalService : IIndicadorAmbientalService
{
    private readonly IIndicadorAmbientalRepository _indicadorRepository;
    private readonly IAreaMonitoradaRepository _areaRepository;
    private readonly IMapper _mapper;

    public IndicadorAmbientalService(
        IIndicadorAmbientalRepository indicadorRepository,
        IAreaMonitoradaRepository areaRepository,
        IMapper mapper)
    {
        _indicadorRepository = indicadorRepository;
        _areaRepository = areaRepository;
        _mapper = mapper;
    }

    public async Task<PagedResultViewModel<IndicadorAmbientalViewModel>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        int? areaMonitoradaId = null,
        TipoIndicador? tipoIndicador = null)
    {
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        pageSize = pageSize <= 0 ? 10 : pageSize;
        pageSize = pageSize > 100 ? 100 : pageSize;

        var (items, totalItems) = await _indicadorRepository.GetPagedAsync(
            pageNumber,
            pageSize,
            areaMonitoradaId,
            tipoIndicador);

        var viewModels = _mapper.Map<IEnumerable<IndicadorAmbientalViewModel>>(items);

        return new PagedResultViewModel<IndicadorAmbientalViewModel>
        {
            Items = viewModels,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }

    public async Task<IndicadorAmbientalViewModel?> GetByIdAsync(int id)
    {
        var indicadorAmbiental = await _indicadorRepository.GetByIdAsync(id);

        if (indicadorAmbiental is null)
        {
            return null;
        }

        return _mapper.Map<IndicadorAmbientalViewModel>(indicadorAmbiental);
    }

    public async Task<IndicadorAmbientalViewModel?> CreateAsync(IndicadorAmbientalCreateViewModel viewModel)
    {
        var areaExiste = await _areaRepository.ExistsAsync(viewModel.AreaMonitoradaId);

        if (!areaExiste)
        {
            return null;
        }

        var indicadorAmbiental = _mapper.Map<IndicadorAmbiental>(viewModel);

        indicadorAmbiental.DataLeitura = DateTime.UtcNow;

        var createdIndicador = await _indicadorRepository.AddAsync(indicadorAmbiental);

        var indicadorCompleto = await _indicadorRepository.GetByIdAsync(createdIndicador.Id);

        return _mapper.Map<IndicadorAmbientalViewModel>(indicadorCompleto);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _indicadorRepository.DeleteAsync(id);
    }
}