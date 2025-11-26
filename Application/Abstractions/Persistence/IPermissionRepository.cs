using DocumentManagement.Domain.Entities;

namespace DocumentManagement.Application.Abstractions.Persistence;

public interface IPermissionRepository
{
    Task<Permission?> GetByFolderAndUserAsync(Guid folderId, Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(Permission permission, CancellationToken cancellationToken = default);
    Task RemoveAsync(Guid permissionId, CancellationToken cancellationToken = default);
    Task UpdateAsync(Permission permission, CancellationToken cancellationToken = default);
}
