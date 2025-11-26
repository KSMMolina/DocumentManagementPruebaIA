using DocumentManagement.Application.Abstractions.Persistence;
using DocumentManagement.Domain.Entities;

namespace DocumentManagement.Infrastructure.Repositories;

public class InMemoryAuditRepository : IAuditRepository
{
    private readonly List<AuditEntry> _entries = new();

    public Task AddAsync(AuditEntry entry, CancellationToken cancellationToken = default)
    {
        _entries.Add(entry);
        return Task.CompletedTask;
    }
}
