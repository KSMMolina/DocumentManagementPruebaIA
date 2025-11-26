using DocumentManagement.Domain.Entities;

namespace DocumentManagement.Application.Abstractions.Persistence;

public interface IDocumentFileRepository
{
    Task<DocumentFile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(DocumentFile file, CancellationToken cancellationToken = default);
    Task UpdateAsync(DocumentFile file, CancellationToken cancellationToken = default);
    Task<int> CountByFolderAsync(Guid folderId, CancellationToken cancellationToken = default);
}
