using Severina.Domain.Entities;
using Severina.Domain.Enums;

namespace Severina.Domain.Interfaces;

public interface IAppointmentRepository : IRepository<Appointment>
{
    Task<IReadOnlyList<Appointment>> GetByDateRangeAsync(Guid companyId, DateTime from, DateTime to);
    Task<IReadOnlyList<Appointment>> GetByClientIdAsync(Guid companyId, Guid clientId);
    Task<IReadOnlyList<Appointment>> GetByStatusAsync(Guid companyId, StatusCompromisso status);
    Task<IReadOnlyList<Appointment>> GetByCompanyAndDateRangeAsync(Guid companyId, DateTime from, DateTime to, int skip, int take);
    Task<int> CountByCompanyAndDateRangeAsync(Guid companyId, DateTime from, DateTime to);
    Task<IReadOnlyList<Appointment>> GetConflictingAsync(Guid companyId, DateTime inicio, DateTime fim, Guid? excludeId = null);
}
