
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TerraPulse.Api.Models.Enums;
using TerraPulse.Api.Services;
using TerraPulse.Api.ViewModels.Common;
using TerraPulse.Api.ViewModels.IndicadoresAmbientais;

namespace TerraPulse.Api.Controllers;

[ApiController]
[Route("api/indicadores-ambientais")]
public class IndicadoresAmbientaisController : ControllerBase
{
    private readonly IIndicadorAmbientalService _service;

    public IndicadoresAmbientaisController(IIndicadorAmbientalService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResultViewModel<IndicadorAmbientalViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultViewModel<IndicadorAmbientalViewModel>>> GetPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? areaMonitoradaId = null,
        [FromQuery] TipoIndicador? tipoIndicador = null)
    {
        var result = await _service.GetPagedAsync(
            pageNumber,
            pageSize,
            areaMonitoradaId,
            tipoIndicador);

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(IndicadorAmbientalViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IndicadorAmbientalViewModel>> GetById(int id)
    {
        var indicadorAmbiental = await _service.GetByIdAsync(id);

        if (indicadorAmbiental is null)
        {
            return NotFound(new { message = "Indicador ambiental não encontrado." });
        }

        return Ok(indicadorAmbiental);
    }

    [Authorize(Roles = "Administrador,Analista")]
    [HttpPost]
    [ProducesResponseType(typeof(IndicadorAmbientalViewModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IndicadorAmbientalViewModel>> Create(
        [FromBody] IndicadorAmbientalCreateViewModel viewModel)
    {
        var indicadorAmbiental = await _service.CreateAsync(viewModel);

        if (indicadorAmbiental is null)
        {
            return BadRequest(new { message = "Área monitorada não encontrada para vincular o indicador." });
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = indicadorAmbiental.Id },
            indicadorAmbiental);
    }

    [Authorize(Roles = "Administrador")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new { message = "Indicador ambiental não encontrado." });
        }

        return NoContent();
    }
}