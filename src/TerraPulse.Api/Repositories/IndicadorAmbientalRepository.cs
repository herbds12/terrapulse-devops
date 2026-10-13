using Microsoft.EntityFrameworkCore;
using TerraPulse.Api.Data;
using TerraPulse.Api.Models;
using TerraPulse.Api.Models.Enums;

namespace TerraPulse.Api.Repositories;

public class IndicadorAmbientalRepository : IIndicadorAmbientalRepository
{
    private readonly TerraPulseDbContext _context;

    public IndicadorAmbientalRepository(TerraPulseDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<IndicadorAmbiental> Items, int TotalItems)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        int? areaMonitoradaId = null,
        TipoIndicador? tipoIndicador = null)
    {
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        pageSize = pageSize <= 0 ? 10 : pageSize;
        pageSize = pageSize > 100 ? 100 : pageSize;

        var query = _context.IndicadoresAmbientais
            .AsNoTracking()
            .Include(x => x.AreaMonitorada)
            .AsQueryable();

        if (areaMonitoradaId.HasValue)
        {
            query = query.Where(x => x.AreaMonitoradaId == areaMonitoradaId.Value);
        }

        if (tipoIndicador.HasValue)
        {
            query = query.Where(x => x.TipoIndicador == tipoIndicador.Value);
        }

        var totalItems = await query.CountAsync();

        var items = await query
            .OrderByDescending(x => x.DataLeitura)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalItems);
    }

    public async Task<IndicadorAmbiental?> GetByIdAsync(int id)
    {
        return await _context.IndicadoresAmbientais
            .AsNoTracking()
            .Include(x => x.AreaMonitorada)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IndicadorAmbiental> AddAsync(IndicadorAmbiental indicadorAmbiental)
    {
        await _context.IndicadoresAmbientais.AddAsync(indicadorAmbiental);
        await _context.SaveChangesAsync();

        return indicadorAmbiental;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var indicadorAmbiental = await _context.IndicadoresAmbientais.FindAsync(id);

        if (indicadorAmbiental is null)
        {
            return false;
        }

        _context.IndicadoresAmbientais.Remove(indicadorAmbiental);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.IndicadoresAmbientais
            .AnyAsync(x => x.Id == id);
    }
}