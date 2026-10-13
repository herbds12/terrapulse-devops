
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TerraPulse.Api.Models.Enums;
using TerraPulse.Api.Services;
using TerraPulse.Api.ViewModels.AreasMonitoradas;
using TerraPulse.Api.ViewModels.Common;

namespace TerraPulse.Api.Controllers;

[ApiController]
[Route("api/areas-monitoradas")]
public class AreasMonitoradasController : ControllerBase
{
    private readonly IAreaMonitoradaService _service;

    public AreasMonitoradasController(IAreaMonitoradaService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResultViewModel<AreaMonitoradaViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultViewModel<AreaMonitoradaViewModel>>> GetPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] StatusArea? status = null,
        [FromQuery] TipoArea? tipoArea = null)
    {
        var result = await _service.GetPagedAsync(pageNumber, pageSize, status, tipoArea);

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AreaMonitoradaViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AreaMonitoradaViewModel>> GetById(int id)
    {
        var areaMonitorada = await _service.GetByIdAsync(id);

        if (areaMonitorada is null)
        {
            return NotFound(new { message = "Área monitorada não encontrada." });
        }

        return Ok(areaMonitorada);
    }

    [Authorize(Roles = "Administrador,Analista")]
    [HttpPost]
    [ProducesResponseType(typeof(AreaMonitoradaViewModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AreaMonitoradaViewModel>> Create(
        [FromBody] AreaMonitoradaCreateViewModel viewModel)
    {
        var areaMonitorada = await _service.CreateAsync(viewModel);

        return CreatedAtAction(
            nameof(GetById),
            new { id = areaMonitorada.Id },
            areaMonitorada);
    }

    [Authorize(Roles = "Administrador,Analista")]
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(AreaMonitoradaViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AreaMonitoradaViewModel>> Update(
        int id,
        [FromBody] AreaMonitoradaUpdateViewModel viewModel)
    {
        var areaMonitorada = await _service.UpdateAsync(id, viewModel);

        if (areaMonitorada is null)
        {
            return NotFound(new { message = "Área monitorada não encontrada." });
        }

        return Ok(areaMonitorada);
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
            return NotFound(new { message = "Área monitorada não encontrada." });
        }

        return NoContent();
    }
}