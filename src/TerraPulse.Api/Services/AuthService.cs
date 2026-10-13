using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TerraPulse.Api.Models;
using TerraPulse.Api.Repositories;
using TerraPulse.Api.ViewModels.Auth;

namespace TerraPulse.Api.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IConfiguration _configuration;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IConfiguration configuration)
    {
        _usuarioRepository = usuarioRepository;
        _configuration = configuration;
    }

    public async Task<TokenViewModel?> LoginAsync(LoginViewModel viewModel)
    {
        var usuario = await _usuarioRepository.GetByEmailAsync(viewModel.Email);

        if (usuario is null)
        {
            return null;
        }

        var senhaValida = PasswordHasher.Verify(viewModel.Senha, usuario.SenhaHash);

        if (!senhaValida)
        {
            return null;
        }

        return GerarToken(usuario);
    }

    private TokenViewModel GerarToken(Usuario usuario)
    {
        var jwtKey = _configuration["Jwt:Key"];
        var jwtIssuer = _configuration["Jwt:Issuer"];
        var jwtAudience = _configuration["Jwt:Audience"];

        if (string.IsNullOrWhiteSpace(jwtKey))
        {
            throw new InvalidOperationException("A chave JWT não foi configurada.");
        }

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var expiresAt = DateTime.UtcNow.AddHours(2);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Perfil.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return new TokenViewModel
        {
            AccessToken = accessToken,
            TokenType = "Bearer",
            ExpiresAt = expiresAt,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Perfil = usuario.Perfil.ToString()
        };
    }
}