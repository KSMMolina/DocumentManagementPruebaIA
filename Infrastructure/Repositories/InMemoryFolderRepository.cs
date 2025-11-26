using DocumentManagement.Application.Abstractions.Persistence;
using DocumentManagement.Domain.Entities;

namespace DocumentManagement.Infrastructure.Repositories;

public class InMemoryFolderRepository : IFolderRepository
{
    private readonly Dictionary<Guid, Folder> _storage = new();

    public Task AddAsync(Folder folder, CancellationToken cancellationToken = default)
    {
        _storage[folder.Id] = folder;
        return Task.CompletedTask;
    }

    public Task<Folder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _storage.TryGetValue(id, out var folder);
        return Task.FromResult(folder);
    }

    public Task<Folder?> GetRootByPropertyAsync(Guid propertyId, CancellationToken cancellationToken = default)
    {
        var folder = _storage.Values.FirstOrDefault(f => f.PropertyId == propertyId && f.ParentFolderId == null);
        return Task.FromResult(folder);
    }

    public Task UpdateAsync(Folder folder, CancellationToken cancellationToken = default)
    {
        _storage[folder.Id] = folder;
        return Task.CompletedTask;
    }
}
