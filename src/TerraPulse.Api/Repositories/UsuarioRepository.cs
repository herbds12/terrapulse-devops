using Microsoft.EntityFrameworkCore;
using TerraPulse.Api.Data;
using TerraPulse.Api.Models;

namespace TerraPulse.Api.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly TerraPulseDbContext _context;

    public UsuarioRepository(TerraPulseDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        return await _context.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == email && x.Ativo);
    }
}