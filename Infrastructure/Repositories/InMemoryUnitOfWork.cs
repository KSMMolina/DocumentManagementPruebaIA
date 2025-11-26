using DocumentManagement.Application.Abstractions.Persistence;

namespace DocumentManagement.Infrastructure.Repositories;

public class InMemoryUnitOfWork : IUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}
