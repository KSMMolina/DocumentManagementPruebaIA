using DocumentManagement.Domain.Entities;

namespace DocumentManagement.Application.Abstractions.Persistence;

public interface IAuditRepository
{
    Task AddAsync(AuditEntry entry, CancellationToken cancellationToken = default);
}
