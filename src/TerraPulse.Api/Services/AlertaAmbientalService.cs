using AutoMapper;
using TerraPulse.Api.Models;
using TerraPulse.Api.Models.Enums;
using TerraPulse.Api.Repositories;
using TerraPulse.Api.ViewModels.AlertasAmbientais;
using TerraPulse.Api.ViewModels.Common;

namespace TerraPulse.Api.Services;

public class AlertaAmbientalService : IAlertaAmbientalService
{
    private readonly IAlertaAmbientalRepository _alertaRepository;
    private readonly IAreaMonitoradaRepository _areaRepository;
    private readonly IIndicadorAmbientalRepository _indicadorRepository;
    private readonly IMapper _mapper;

    public AlertaAmbientalService(
        IAlertaAmbientalRepository alertaRepository,
        IAreaMonitoradaRepository areaRepository,
        IIndicadorAmbientalRepository indicadorRepository,
        IMapper mapper)
    {
        _alertaRepository = alertaRepository;
        _areaRepository = areaRepository;
        _indicadorRepository = indicadorRepository;
        _mapper = mapper;
    }

    public async Task<PagedResultViewModel<AlertaAmbientalViewModel>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        int? areaMonitoradaId = null,
        StatusAlerta? status = null,
        SeveridadeAlerta? severidade = null)
    {
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        pageSize = pageSize <= 0 ? 10 : pageSize;
        pageSize = pageSize > 100 ? 100 : pageSize;

        var (items, totalItems) = await _alertaRepository.GetPagedAsync(
            pageNumber,
            pageSize,
            areaMonitoradaId,
            status,
            severidade);

        var viewModels = _mapper.Map<IEnumerable<AlertaAmbientalViewModel>>(items);

        return new PagedResultViewModel<AlertaAmbientalViewModel>
        {
            Items = viewModels,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }

    public async Task<AlertaAmbientalViewModel?> GetByIdAsync(int id)
    {
        var alerta = await _alertaRepository.GetByIdAsync(id);

        if (alerta is null)
        {
            return null;
        }

        return _mapper.Map<AlertaAmbientalViewModel>(alerta);
    }

    public async Task<IEnumerable<AlertaAmbientalViewModel>?> GerarAlertasAsync(
        AlertaAmbientalGerarViewModel viewModel)
    {
        var area = await _areaRepository.GetByIdAsync(viewModel.AreaMonitoradaId);

        if (area is null)
        {
            return null;
        }

        var (indicadores, _) = await _indicadorRepository.GetPagedAsync(
            pageNumber: 1,
            pageSize: 100,
            areaMonitoradaId: viewModel.AreaMonitoradaId);

        var ultimosIndicadoresPorTipo = indicadores
            .GroupBy(x => x.TipoIndicador)
            .Select(x => x.OrderByDescending(i => i.DataLeitura).First())
            .ToList();

        var alertas = new List<AlertaAmbiental>();

        foreach (var indicador in ultimosIndicadoresPorTipo)
        {
            var alerta = CriarAlertaComBaseNoIndicador(indicador);

            if (alerta is not null)
            {
                alerta.AreaMonitoradaId = viewModel.AreaMonitoradaId;
                alerta.DataGeracao = DateTime.UtcNow;
                alerta.Status = StatusAlerta.Aberto;

                alertas.Add(alerta);
            }
        }

        if (!alertas.Any())
        {
            return new List<AlertaAmbientalViewModel>();
        }

        var alertasCriados = await _alertaRepository.AddRangeAsync(alertas);

        foreach (var alerta in alertasCriados)
        {
            alerta.AreaMonitorada = area;
        }

        return _mapper.Map<IEnumerable<AlertaAmbientalViewModel>>(alertasCriados);
    }

    public async Task<AlertaAmbientalViewModel?> AtualizarStatusAsync(
        int id,
        AlertaAmbientalStatusUpdateViewModel viewModel)
    {
        var alerta = await _alertaRepository.GetByIdAsync(id);

        if (alerta is null)
        {
            return null;
        }

        alerta.Status = viewModel.Status;

        if (viewModel.Status == StatusAlerta.Resolvido)
        {
            alerta.DataResolucao = DateTime.UtcNow;
        }
        else
        {
            alerta.DataResolucao = null;
        }

        var areaMonitorada = alerta.AreaMonitorada;
        alerta.AreaMonitorada = null;

        var alertaAtualizado = await _alertaRepository.UpdateAsync(alerta);
        alertaAtualizado.AreaMonitorada = areaMonitorada;

        return _mapper.Map<AlertaAmbientalViewModel>(alertaAtualizado);
    }

    private static AlertaAmbiental? CriarAlertaComBaseNoIndicador(IndicadorAmbiental indicador)
    {
        return indicador.TipoIndicador switch
        {
            TipoIndicador.UmidadeSolo when indicador.Valor < 20 =>
                new AlertaAmbiental
                {
                    TipoAlerta = TipoAlerta.Seca,
                    Severidade = indicador.Valor < 15 ? SeveridadeAlerta.Critica : SeveridadeAlerta.Alta,
                    Titulo = "Risco de seca identificado",
                    Descricao = $"A umidade do solo está em {indicador.Valor}{indicador.UnidadeMedida}, abaixo do limite recomendado.",
                    NivelConfianca = 87.5m
                },

            TipoIndicador.EstresseHidrico when indicador.Valor >= 80 =>
                new AlertaAmbiental
                {
                    TipoAlerta = TipoAlerta.EstresseHidrico,
                    Severidade = indicador.Valor >= 90 ? SeveridadeAlerta.Critica : SeveridadeAlerta.Alta,
                    Titulo = "Estresse hídrico elevado",
                    Descricao = $"O índice de estresse hídrico atingiu {indicador.Valor}{indicador.UnidadeMedida}, indicando risco para a área monitorada.",
                    NivelConfianca = 91.2m
                },

            TipoIndicador.SaudeVegetacao when indicador.Valor < 50 =>
                new AlertaAmbiental
                {
                    TipoAlerta = TipoAlerta.QuedaVegetacao,
                    Severidade = indicador.Valor < 35 ? SeveridadeAlerta.Alta : SeveridadeAlerta.Media,
                    Titulo = "Queda na saúde da vegetação",
                    Descricao = $"O indicador de saúde da vegetação está em {indicador.Valor}, abaixo do padrão esperado.",
                    NivelConfianca = 84.3m
                },

            TipoIndicador.RiscoDesmatamento when indicador.Valor >= 70 =>
                new AlertaAmbiental
                {
                    TipoAlerta = TipoAlerta.RiscoDesmatamento,
                    Severidade = indicador.Valor >= 90 ? SeveridadeAlerta.Critica : SeveridadeAlerta.Alta,
                    Titulo = "Risco de desmatamento detectado",
                    Descricao = $"O risco de desmatamento foi estimado em {indicador.Valor}{indicador.UnidadeMedida}.",
                    NivelConfianca = 88.9m
                },

            TipoIndicador.CarbonoEstimado when indicador.Valor >= 100 =>
                new AlertaAmbiental
                {
                    TipoAlerta = TipoAlerta.EmissaoCarbonoElevada,
                    Severidade = SeveridadeAlerta.Media,
                    Titulo = "Emissão de carbono elevada",
                    Descricao = $"A emissão estimada de carbono atingiu {indicador.Valor}{indicador.UnidadeMedida}.",
                    NivelConfianca = 79.6m
                },

            TipoIndicador.RiscoClimatico when indicador.Valor >= 75 =>
                new AlertaAmbiental
                {
                    TipoAlerta = TipoAlerta.ChuvaIntensa,
                    Severidade = indicador.Valor >= 90 ? SeveridadeAlerta.Critica : SeveridadeAlerta.Alta,
                    Titulo = "Risco climático elevado",
                    Descricao = $"O risco climático foi calculado em {indicador.Valor}{indicador.UnidadeMedida}.",
                    NivelConfianca = 86.4m
                },

            _ => null
        };
    }
}