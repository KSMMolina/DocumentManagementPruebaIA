using DocumentManagement.Application.Abstractions.Persistence;
using DocumentManagement.Domain.Entities;

namespace DocumentManagement.Infrastructure.Repositories;

public class InMemoryPermissionRepository : IPermissionRepository
{
    private readonly Dictionary<Guid, Permission> _storage = new();

    public Task AddAsync(Permission permission, CancellationToken cancellationToken = default)
    {
        _storage[permission.Id] = permission;
        return Task.CompletedTask;
    }

    public Task<Permission?> GetByFolderAndUserAsync(Guid folderId, Guid userId, CancellationToken cancellationToken = default)
    {
        var permission = _storage.Values.FirstOrDefault(p => p.FolderId == folderId && p.UserId == userId);
        return Task.FromResult(permission);
    }

    public Task RemoveAsync(Guid permissionId, CancellationToken cancellationToken = default)
    {
        _storage.Remove(permissionId);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Permission permission, CancellationToken cancellationToken = default)
    {
        _storage[permission.Id] = permission;
        return Task.CompletedTask;
    }
}
