using Microsoft.EntityFrameworkCore;
using TerraPulse.Api.Data;
using TerraPulse.Api.Models;
using TerraPulse.Api.Models.Enums;

namespace TerraPulse.Api.Repositories;

public class AlertaAmbientalRepository : IAlertaAmbientalRepository
{
    private readonly TerraPulseDbContext _context;

    public AlertaAmbientalRepository(TerraPulseDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<AlertaAmbiental> Items, int TotalItems)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        int? areaMonitoradaId = null,
        StatusAlerta? status = null,
        SeveridadeAlerta? severidade = null)
    {
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        pageSize = pageSize <= 0 ? 10 : pageSize;
        pageSize = pageSize > 100 ? 100 : pageSize;

        var query = _context.AlertasAmbientais
            .AsNoTracking()
            .Include(x => x.AreaMonitorada)
            .AsQueryable();

        if (areaMonitoradaId.HasValue)
        {
            query = query.Where(x => x.AreaMonitoradaId == areaMonitoradaId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        if (severidade.HasValue)
        {
            query = query.Where(x => x.Severidade == severidade.Value);
        }

        var totalItems = await query.CountAsync();

        var items = await query
            .OrderByDescending(x => x.DataGeracao)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalItems);
    }

    public async Task<AlertaAmbiental?> GetByIdAsync(int id)
    {
        return await _context.AlertasAmbientais
            .AsNoTracking()
            .Include(x => x.AreaMonitorada)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<AlertaAmbiental> AddAsync(AlertaAmbiental alertaAmbiental)
    {
        await _context.AlertasAmbientais.AddAsync(alertaAmbiental);
        await _context.SaveChangesAsync();

        return alertaAmbiental;
    }

    public async Task<IEnumerable<AlertaAmbiental>> AddRangeAsync(IEnumerable<AlertaAmbiental> alertasAmbientais)
    {
        var alertas = alertasAmbientais.ToList();

        await _context.AlertasAmbientais.AddRangeAsync(alertas);
        await _context.SaveChangesAsync();

        return alertas;
    }

    public async Task<AlertaAmbiental> UpdateAsync(AlertaAmbiental alertaAmbiental)
    {
        _context.AlertasAmbientais.Update(alertaAmbiental);
        await _context.SaveChangesAsync();

        return alertaAmbiental;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.AlertasAmbientais
            .AnyAsync(x => x.Id == id);
    }
}