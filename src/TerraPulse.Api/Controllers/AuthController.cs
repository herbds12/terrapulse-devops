using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TerraPulse.Api.Services;
using TerraPulse.Api.ViewModels.Auth;

namespace TerraPulse.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(TokenViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TokenViewModel>> Login([FromBody] LoginViewModel viewModel)
    {
        var token = await _authService.LoginAsync(viewModel);

        if (token is null)
        {
            return Unauthorized(new { message = "E-mail ou senha inválidos." });
        }

        return Ok(token);
    }
}