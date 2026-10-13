using Microsoft.EntityFrameworkCore;
using TerraPulse.Api.Data;
using TerraPulse.Api.Models;
using TerraPulse.Api.Models.Enums;

namespace TerraPulse.Api.Repositories;

public class RecomendacaoSustentavelRepository : IRecomendacaoSustentavelRepository
{
    private readonly TerraPulseDbContext _context;

    public RecomendacaoSustentavelRepository(TerraPulseDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<RecomendacaoSustentavel> Items, int TotalItems)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        int? alertaAmbientalId = null,
        StatusRecomendacao? status = null)
    {
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        pageSize = pageSize <= 0 ? 10 : pageSize;
        pageSize = pageSize > 100 ? 100 : pageSize;

        var query = _context.RecomendacoesSustentaveis
            .AsNoTracking()
            .Include(x => x.AlertaAmbiental)
                .ThenInclude(x => x!.AreaMonitorada)
            .AsQueryable();

        if (alertaAmbientalId.HasValue)
        {
            query = query.Where(x => x.AlertaAmbientalId == alertaAmbientalId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        var totalItems = await query.CountAsync();

        var items = await query
            .OrderByDescending(x => x.DataGeracao)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalItems);
    }

    public async Task<RecomendacaoSustentavel?> GetByIdAsync(int id)
    {
        return await _context.RecomendacoesSustentaveis
            .AsNoTracking()
            .Include(x => x.AlertaAmbiental)
                .ThenInclude(x => x!.AreaMonitorada)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<RecomendacaoSustentavel> AddAsync(RecomendacaoSustentavel recomendacaoSustentavel)
    {
        await _context.RecomendacoesSustentaveis.AddAsync(recomendacaoSustentavel);
        await _context.SaveChangesAsync();

        return recomendacaoSustentavel;
    }

    public async Task<RecomendacaoSustentavel> UpdateAsync(RecomendacaoSustentavel recomendacaoSustentavel)
    {
        _context.RecomendacoesSustentaveis.Update(recomendacaoSustentavel);
        await _context.SaveChangesAsync();

        return recomendacaoSustentavel;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.RecomendacoesSustentaveis
            .AnyAsync(x => x.Id == id);
    }
}