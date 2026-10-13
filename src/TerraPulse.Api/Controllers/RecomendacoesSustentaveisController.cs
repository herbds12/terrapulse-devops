
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TerraPulse.Api.Models.Enums;
using TerraPulse.Api.Services;
using TerraPulse.Api.ViewModels.Common;
using TerraPulse.Api.ViewModels.RecomendacoesSustentaveis;

namespace TerraPulse.Api.Controllers;

[ApiController]
[Route("api/recomendacoes-sustentaveis")]
public class RecomendacoesSustentaveisController : ControllerBase
{
    private readonly IRecomendacaoSustentavelService _service;

    public RecomendacoesSustentaveisController(IRecomendacaoSustentavelService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResultViewModel<RecomendacaoSustentavelViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultViewModel<RecomendacaoSustentavelViewModel>>> GetPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? alertaAmbientalId = null,
        [FromQuery] StatusRecomendacao? status = null)
    {
        var result = await _service.GetPagedAsync(
            pageNumber,
            pageSize,
            alertaAmbientalId,
            status);

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(RecomendacaoSustentavelViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecomendacaoSustentavelViewModel>> GetById(int id)
    {
        var recomendacao = await _service.GetByIdAsync(id);

        if (recomendacao is null)
        {
            return NotFound(new { message = "Recomendação sustentável não encontrada." });
        }

        return Ok(recomendacao);
    }

    [Authorize(Roles = "Administrador,Analista")]
    [HttpPost("gerar")]
    [ProducesResponseType(typeof(RecomendacaoSustentavelViewModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RecomendacaoSustentavelViewModel>> GerarRecomendacao(
        [FromBody] RecomendacaoSustentavelGerarViewModel viewModel)
    {
        var recomendacao = await _service.GerarRecomendacaoAsync(viewModel);

        if (recomendacao is null)
        {
            return BadRequest(new { message = "Alerta ambiental não encontrado para geração da recomendação." });
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = recomendacao.Id },
            recomendacao);
    }

    [Authorize(Roles = "Administrador,Analista")]
    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(typeof(RecomendacaoSustentavelViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RecomendacaoSustentavelViewModel>> AtualizarStatus(
        int id,
        [FromBody] RecomendacaoSustentavelStatusUpdateViewModel viewModel)
    {
        var recomendacaoAtualizada = await _service.AtualizarStatusAsync(id, viewModel);

        if (recomendacaoAtualizada is null)
        {
            return NotFound(new { message = "Recomendação sustentável não encontrada." });
        }

        return Ok(recomendacaoAtualizada);
    }
}