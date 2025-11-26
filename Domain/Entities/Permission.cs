using DocumentManagement.Domain.Common;
using DocumentManagement.Domain.Enums;

namespace DocumentManagement.Domain.Entities;

public sealed class Permission : Entity<Guid>
{
    public Guid FolderId { get; }
    public Guid UserId { get; }
    public PermissionType Access { get; private set; }

    private Permission() : base(Guid.Empty)
    {
        FolderId = Guid.Empty;
        UserId = Guid.Empty;
    }

    public Permission(Guid id, Guid folderId, Guid userId, PermissionType access) : base(id)
    {
        FolderId = folderId;
        UserId = userId;
        Access = access;
    }

    public void UpdateAccess(PermissionType newAccess)
    {
        Access = newAccess;
    }
}
