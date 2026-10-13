using AutoMapper;
using TerraPulse.Api.Models;
using TerraPulse.Api.Models.Enums;
using TerraPulse.Api.Repositories;
using TerraPulse.Api.ViewModels.AreasMonitoradas;
using TerraPulse.Api.ViewModels.Common;

namespace TerraPulse.Api.Services;

public class AreaMonitoradaService : IAreaMonitoradaService
{
    private readonly IAreaMonitoradaRepository _repository;
    private readonly IMapper _mapper;

    public AreaMonitoradaService(
        IAreaMonitoradaRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PagedResultViewModel<AreaMonitoradaViewModel>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        StatusArea? status = null,
        TipoArea? tipoArea = null)
    {
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        pageSize = pageSize <= 0 ? 10 : pageSize;
        pageSize = pageSize > 100 ? 100 : pageSize;

        var (items, totalItems) = await _repository.GetPagedAsync(
            pageNumber,
            pageSize,
            status,
            tipoArea);

        var viewModels = _mapper.Map<IEnumerable<AreaMonitoradaViewModel>>(items);

        return new PagedResultViewModel<AreaMonitoradaViewModel>
        {
            Items = viewModels,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }

    public async Task<AreaMonitoradaViewModel?> GetByIdAsync(int id)
    {
        var areaMonitorada = await _repository.GetByIdAsync(id);

        if (areaMonitorada is null)
        {
            return null;
        }

        return _mapper.Map<AreaMonitoradaViewModel>(areaMonitorada);
    }

    public async Task<AreaMonitoradaViewModel> CreateAsync(AreaMonitoradaCreateViewModel viewModel)
    {
        var areaMonitorada = _mapper.Map<AreaMonitorada>(viewModel);

        areaMonitorada.Status = StatusArea.Ativa;
        areaMonitorada.DataCadastro = DateTime.UtcNow;
        areaMonitorada.UltimaSincronizacao = DateTime.UtcNow;

        var createdArea = await _repository.AddAsync(areaMonitorada);

        return _mapper.Map<AreaMonitoradaViewModel>(createdArea);
    }

    public async Task<AreaMonitoradaViewModel?> UpdateAsync(int id, AreaMonitoradaUpdateViewModel viewModel)
    {
        var areaMonitorada = await _repository.GetByIdAsync(id);

        if (areaMonitorada is null)
        {
            return null;
        }

        areaMonitorada.Nome = viewModel.Nome;
        areaMonitorada.Pais = viewModel.Pais;
        areaMonitorada.Regiao = viewModel.Regiao;
        areaMonitorada.Cultura = viewModel.Cultura;
        areaMonitorada.TipoArea = viewModel.TipoArea;
        areaMonitorada.Status = viewModel.Status;
        areaMonitorada.TamanhoHectares = viewModel.TamanhoHectares;
        areaMonitorada.Latitude = viewModel.Latitude;
        areaMonitorada.Longitude = viewModel.Longitude;
        areaMonitorada.UltimaSincronizacao = DateTime.UtcNow;

        var updatedArea = await _repository.UpdateAsync(areaMonitorada);

        return _mapper.Map<AreaMonitoradaViewModel>(updatedArea);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }
}