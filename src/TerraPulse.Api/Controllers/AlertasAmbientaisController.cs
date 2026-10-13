
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TerraPulse.Api.Models.Enums;
using TerraPulse.Api.Services;
using TerraPulse.Api.ViewModels.AlertasAmbientais;
using TerraPulse.Api.ViewModels.Common;

namespace TerraPulse.Api.Controllers;

[ApiController]
[Route("api/alertas-ambientais")]
public class AlertasAmbientaisController : ControllerBase
{
    private readonly IAlertaAmbientalService _service;

    public AlertasAmbientaisController(IAlertaAmbientalService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResultViewModel<AlertaAmbientalViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultViewModel<AlertaAmbientalViewModel>>> GetPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? areaMonitoradaId = null,
        [FromQuery] StatusAlerta? status = null,
        [FromQuery] SeveridadeAlerta? severidade = null)
    {
        var result = await _service.GetPagedAsync(
            pageNumber,
            pageSize,
            areaMonitoradaId,
            status,
            severidade);

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AlertaAmbientalViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AlertaAmbientalViewModel>> GetById(int id)
    {
        var alertaAmbiental = await _service.GetByIdAsync(id);

        if (alertaAmbiental is null)
        {
            return NotFound(new { message = "Alerta ambiental não encontrado." });
        }

        return Ok(alertaAmbiental);
    }

    [Authorize(Roles = "Administrador,Analista")]
    [HttpPost("gerar")]
    [ProducesResponseType(typeof(IEnumerable<AlertaAmbientalViewModel>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<AlertaAmbientalViewModel>>> GerarAlertas(
        [FromBody] AlertaAmbientalGerarViewModel viewModel)
    {
        var alertasGerados = await _service.GerarAlertasAsync(viewModel);

        if (alertasGerados is null)
        {
            return BadRequest(new { message = "Área monitorada não encontrada para geração de alertas." });
        }

        return Created(
            "/api/alertas-ambientais",
            alertasGerados);
    }

    [Authorize(Roles = "Administrador,Analista")]
    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(typeof(AlertaAmbientalViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AlertaAmbientalViewModel>> AtualizarStatus(
        int id,
        [FromBody] AlertaAmbientalStatusUpdateViewModel viewModel)
    {
        var alertaAtualizado = await _service.AtualizarStatusAsync(id, viewModel);

        if (alertaAtualizado is null)
        {
            return NotFound(new { message = "Alerta ambiental não encontrado." });
        }

        return Ok(alertaAtualizado);
    }
}