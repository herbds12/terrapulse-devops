using TerraPulse.Api.ViewModels.Auth;

namespace TerraPulse.Api.Services;

public interface IAuthService
{
    Task<TokenViewModel?> LoginAsync(LoginViewModel viewModel);
}