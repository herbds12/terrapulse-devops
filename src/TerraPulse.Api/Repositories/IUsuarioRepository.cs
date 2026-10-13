using TerraPulse.Api.Models;

namespace TerraPulse.Api.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario?> GetByEmailAsync(string email);
}