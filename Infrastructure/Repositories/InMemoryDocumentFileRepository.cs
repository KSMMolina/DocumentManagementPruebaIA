using DocumentManagement.Application.Abstractions.Persistence;
using DocumentManagement.Domain.Entities;

namespace DocumentManagement.Infrastructure.Repositories;

public class InMemoryDocumentFileRepository : IDocumentFileRepository
{
    private readonly Dictionary<Guid, DocumentFile> _storage = new();

    public Task AddAsync(DocumentFile file, CancellationToken cancellationToken = default)
    {
        _storage[file.Id] = file;
        return Task.CompletedTask;
    }

    public Task<int> CountByFolderAsync(Guid folderId, CancellationToken cancellationToken = default)
    {
        var count = _storage.Values.Count(f => f.FolderId == folderId);
        return Task.FromResult(count);
    }

    public Task<DocumentFile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _storage.TryGetValue(id, out var file);
        return Task.FromResult(file);
    }

    public Task UpdateAsync(DocumentFile file, CancellationToken cancellationToken = default)
    {
        _storage[file.Id] = file;
        return Task.CompletedTask;
    }
}
