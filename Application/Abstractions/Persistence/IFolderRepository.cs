using DocumentManagement.Domain.Entities;

namespace DocumentManagement.Application.Abstractions.Persistence;

public interface IFolderRepository
{
    Task<Folder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Folder?> GetRootByPropertyAsync(Guid propertyId, CancellationToken cancellationToken = default);
    Task AddAsync(Folder folder, CancellationToken cancellationToken = default);
    Task UpdateAsync(Folder folder, CancellationToken cancellationToken = default);
}
