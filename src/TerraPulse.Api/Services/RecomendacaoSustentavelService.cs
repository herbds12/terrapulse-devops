using AutoMapper;
using TerraPulse.Api.Models;
using TerraPulse.Api.Models.Enums;
using TerraPulse.Api.Repositories;
using TerraPulse.Api.ViewModels.Common;
using TerraPulse.Api.ViewModels.RecomendacoesSustentaveis;

namespace TerraPulse.Api.Services;

public class RecomendacaoSustentavelService : IRecomendacaoSustentavelService
{
    private readonly IRecomendacaoSustentavelRepository _recomendacaoRepository;
    private readonly IAlertaAmbientalRepository _alertaRepository;
    private readonly IMapper _mapper;

    public RecomendacaoSustentavelService(
        IRecomendacaoSustentavelRepository recomendacaoRepository,
        IAlertaAmbientalRepository alertaRepository,
        IMapper mapper)
    {
        _recomendacaoRepository = recomendacaoRepository;
        _alertaRepository = alertaRepository;
        _mapper = mapper;
    }

    public async Task<PagedResultViewModel<RecomendacaoSustentavelViewModel>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        int? alertaAmbientalId = null,
        StatusRecomendacao? status = null)
    {
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        pageSize = pageSize <= 0 ? 10 : pageSize;
        pageSize = pageSize > 100 ? 100 : pageSize;

        var (items, totalItems) = await _recomendacaoRepository.GetPagedAsync(
            pageNumber,
            pageSize,
            alertaAmbientalId,
            status);

        var viewModels = _mapper.Map<IEnumerable<RecomendacaoSustentavelViewModel>>(items);

        return new PagedResultViewModel<RecomendacaoSustentavelViewModel>
        {
            Items = viewModels,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }

    public async Task<RecomendacaoSustentavelViewModel?> GetByIdAsync(int id)
    {
        var recomendacao = await _recomendacaoRepository.GetByIdAsync(id);

        if (recomendacao is null)
        {
            return null;
        }

        return _mapper.Map<RecomendacaoSustentavelViewModel>(recomendacao);
    }

    public async Task<RecomendacaoSustentavelViewModel?> GerarRecomendacaoAsync(
        RecomendacaoSustentavelGerarViewModel viewModel)
    {
        var alerta = await _alertaRepository.GetByIdAsync(viewModel.AlertaAmbientalId);

        if (alerta is null)
        {
            return null;
        }

        var recomendacao = CriarRecomendacaoComBaseNoAlerta(alerta);

        recomendacao.AlertaAmbientalId = alerta.Id;
        recomendacao.Status = StatusRecomendacao.Pendente;
        recomendacao.DataGeracao = DateTime.UtcNow;

        var recomendacaoCriada = await _recomendacaoRepository.AddAsync(recomendacao);

        var recomendacaoCompleta = await _recomendacaoRepository.GetByIdAsync(recomendacaoCriada.Id);

        return _mapper.Map<RecomendacaoSustentavelViewModel>(recomendacaoCompleta);
    }

    public async Task<RecomendacaoSustentavelViewModel?> AtualizarStatusAsync(
        int id,
        RecomendacaoSustentavelStatusUpdateViewModel viewModel)
    {
        var recomendacao = await _recomendacaoRepository.GetByIdAsync(id);

        if (recomendacao is null)
        {
            return null;
        }

        recomendacao.Status = viewModel.Status;

        if (viewModel.Status == StatusRecomendacao.Aplicada)
        {
            recomendacao.DataAplicacao = DateTime.UtcNow;
        }
        else
        {
            recomendacao.DataAplicacao = null;
        }

        var alertaAmbiental = recomendacao.AlertaAmbiental;
        recomendacao.AlertaAmbiental = null;

        var recomendacaoAtualizada = await _recomendacaoRepository.UpdateAsync(recomendacao);
        recomendacaoAtualizada.AlertaAmbiental = alertaAmbiental;

        return _mapper.Map<RecomendacaoSustentavelViewModel>(recomendacaoAtualizada);
    }

    private static RecomendacaoSustentavel CriarRecomendacaoComBaseNoAlerta(AlertaAmbiental alerta)
    {
        return alerta.TipoAlerta switch
        {
            TipoAlerta.Seca =>
                new RecomendacaoSustentavel
                {
                    Titulo = "Priorizar irrigação emergencial",
                    Descricao = "Executar plano de irrigação controlada na área afetada e monitorar a umidade do solo nas próximas 24 horas.",
                    BeneficioEsperado = "Redução do risco de perda produtiva e uso mais eficiente da água."
                },

            TipoAlerta.EstresseHidrico =>
                new RecomendacaoSustentavel
                {
                    Titulo = "Aplicar plano de eficiência hídrica",
                    Descricao = "Revisar o consumo de água da área, priorizar setores críticos e ajustar a irrigação conforme a previsão climática.",
                    BeneficioEsperado = "Mitigação do estresse hídrico e preservação dos recursos naturais."
                },

            TipoAlerta.QuedaVegetacao =>
                new RecomendacaoSustentavel
                {
                    Titulo = "Investigar queda na saúde da vegetação",
                    Descricao = "Realizar análise de solo, verificar presença de pragas e comparar o índice de vegetação com leituras anteriores.",
                    BeneficioEsperado = "Identificação precoce de degradação ambiental e recuperação da produtividade."
                },

            TipoAlerta.RiscoDesmatamento =>
                new RecomendacaoSustentavel
                {
                    Titulo = "Acionar protocolo de preservação ambiental",
                    Descricao = "Priorizar vistoria da área, registrar evidências geoespaciais e acionar responsáveis pela governança ambiental.",
                    BeneficioEsperado = "Redução do risco de desmatamento e melhoria da conformidade ambiental."
                },

            TipoAlerta.EmissaoCarbonoElevada =>
                new RecomendacaoSustentavel
                {
                    Titulo = "Revisar plano de compensação de carbono",
                    Descricao = "Avaliar fontes de emissão, revisar processos operacionais e planejar ações de compensação ambiental.",
                    BeneficioEsperado = "Redução da pegada de carbono e fortalecimento de práticas ESG."
                },

            TipoAlerta.ChuvaIntensa =>
                new RecomendacaoSustentavel
                {
                    Titulo = "Preparar plano preventivo para evento climático",
                    Descricao = "Verificar drenagem, proteger áreas vulneráveis e acompanhar atualização da previsão climática.",
                    BeneficioEsperado = "Redução de danos operacionais e aumento da resiliência climática."
                },

            _ =>
                new RecomendacaoSustentavel
                {
                    Titulo = "Avaliar alerta ambiental",
                    Descricao = "Analisar o alerta gerado e definir plano de ação adequado com base nos dados ambientais disponíveis.",
                    BeneficioEsperado = "Melhoria da tomada de decisão ambiental."
                }
        };
    }
}