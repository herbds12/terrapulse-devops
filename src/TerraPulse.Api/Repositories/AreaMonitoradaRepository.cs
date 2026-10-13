using Microsoft.EntityFrameworkCore;
using TerraPulse.Api.Data;
using TerraPulse.Api.Models;
using TerraPulse.Api.Models.Enums;

namespace TerraPulse.Api.Repositories;

public class AreaMonitoradaRepository : IAreaMonitoradaRepository
{
    private readonly TerraPulseDbContext _context;

    public AreaMonitoradaRepository(TerraPulseDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<AreaMonitorada> Items, int TotalItems)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        StatusArea? status = null,
        TipoArea? tipoArea = null)
    {
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        pageSize = pageSize <= 0 ? 10 : pageSize;
        pageSize = pageSize > 100 ? 100 : pageSize;

        var query = _context.AreasMonitoradas
            .AsNoTracking()
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        if (tipoArea.HasValue)
        {
            query = query.Where(x => x.TipoArea == tipoArea.Value);
        }

        var totalItems = await query.CountAsync();

        var items = await query
            .OrderBy(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalItems);
    }

    public async Task<AreaMonitorada?> GetByIdAsync(int id)
    {
        return await _context.AreasMonitoradas
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<AreaMonitorada> AddAsync(AreaMonitorada areaMonitorada)
    {
        await _context.AreasMonitoradas.AddAsync(areaMonitorada);
        await _context.SaveChangesAsync();

        return areaMonitorada;
    }

    public async Task<AreaMonitorada> UpdateAsync(AreaMonitorada areaMonitorada)
    {
        _context.AreasMonitoradas.Update(areaMonitorada);
        await _context.SaveChangesAsync();

        return areaMonitorada;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var areaMonitorada = await _context.AreasMonitoradas.FindAsync(id);

        if (areaMonitorada is null)
        {
            return false;
        }

        _context.AreasMonitoradas.Remove(areaMonitorada);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.AreasMonitoradas
            .AnyAsync(x => x.Id == id);
    }
}