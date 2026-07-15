using Microsoft.EntityFrameworkCore;
using Severina.Domain.Entities;
using Severina.Domain.Enums;
using Severina.Domain.Interfaces;
using Severina.Infrastructure.Data;

namespace Severina.Infrastructure.Repositories;

public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(SeverinaDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Appointment>> GetByDateRangeAsync(Guid companyId, DateTime from, DateTime to)
    {
        return await _dbSet
            .Where(a => a.CompanyId == companyId && a.DataHoraInicio >= from && a.DataHoraInicio <= to)
            .OrderBy(a => a.DataHoraInicio)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Appointment>> GetByClientIdAsync(Guid companyId, Guid clientId)
    {
        return await _dbSet
            .Where(a => a.CompanyId == companyId && a.ClientId == clientId)
            .OrderBy(a => a.DataHoraInicio)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Appointment>> GetByStatusAsync(Guid companyId, StatusCompromisso status)
    {
        return await _dbSet
            .Where(a => a.CompanyId == companyId && a.Status == status)
            .OrderBy(a => a.DataHoraInicio)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Appointment>> GetByCompanyAndDateRangeAsync(Guid companyId, DateTime from, DateTime to, int skip, int take)
    {
        return await _dbSet
            .Where(a => a.CompanyId == companyId && a.DataHoraInicio >= from && a.DataHoraInicio <= to)
            .OrderBy(a => a.DataHoraInicio)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<int> CountByCompanyAndDateRangeAsync(Guid companyId, DateTime from, DateTime to)
    {
        return await _dbSet
            .CountAsync(a => a.CompanyId == companyId && a.DataHoraInicio >= from && a.DataHoraInicio <= to);
    }

    public async Task<IReadOnlyList<Appointment>> GetConflictingAsync(Guid companyId, DateTime inicio, DateTime fim, Guid? excludeId = null)
    {
        var query = _dbSet
            .Where(a => a.CompanyId == companyId
                && a.DataHoraInicio < fim
                && a.DataHoraFim > inicio
                && a.Status != StatusCompromisso.Cancelled);

        if (excludeId.HasValue)
            query = query.Where(a => a.Id != excludeId.Value);

        return await query.ToListAsync();
    }
}
