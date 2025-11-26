using DocumentManagement.Domain.Common;
using DocumentManagement.Domain.Enums;
using DocumentManagement.Domain.ValueObjects;

namespace DocumentManagement.Domain.Entities;

public sealed class Folder : Entity<Guid>
{
    private readonly List<Folder> _children = new();
    private readonly List<DocumentFile> _files = new();
    private readonly List<Permission> _permissions = new();

    public Guid PropertyId { get; }
    public Guid? ParentFolderId { get; }
    public FolderName Name { get; private set; }
    public FolderDepth Depth { get; }

    private Folder() : base(Guid.Empty)
    {
        PropertyId = Guid.Empty;
        Name = new FolderName("?");
        Depth = new FolderDepth(1);
    }

    public IReadOnlyCollection<Folder> Children => _children;
    public IReadOnlyCollection<DocumentFile> Files => _files;
    public IReadOnlyCollection<Permission> Permissions => _permissions;

    public Folder(Guid id, Guid propertyId, FolderName name, FolderDepth depth, Guid? parentFolderId = null) : base(id)
    {
        PropertyId = propertyId;
        ParentFolderId = parentFolderId;
        Name = name;
        Depth = depth;
    }

    public Folder AddChildFolder(Guid childId, FolderName name)
    {
        if (Depth.Value >= FolderDepth.MaxDepth)
        {
            throw new DomainException("No se pueden crear más subcarpetas, se alcanzó la profundidad máxima.");
        }

        if (_children.Count >= 2)
        {
            throw new DomainException("Cada carpeta solo puede tener 2 subcarpetas.");
        }

        var child = new Folder(childId, PropertyId, name, new FolderDepth(Depth.Value + 1), Id);
        _children.Add(child);
        return child;
    }

    public DocumentFile AddFile(Guid fileId, FileName fileName, DescriptionText description, FileSize size, DateTime createdAt)
    {
        if (_files.Count >= 5)
        {
            throw new DomainException("No se pueden agregar más de 5 archivos en la carpeta.");
        }

        var file = new DocumentFile(fileId, Id, fileName, description, size, createdAt);
        _files.Add(file);
        return file;
    }

    public void Rename(FolderName newName)
    {
        Name = newName;
    }

    public void AssignPermission(Guid permissionId, Guid userId, PermissionType access, UserRole userRole)
    {
        if (userRole == UserRole.Administrator)
        {
            // Administrador siempre debe mantener control total
            access = PermissionType.View | PermissionType.Download;
        }

        var existing = _permissions.FirstOrDefault(p => p.UserId == userId);
        if (existing is null)
        {
            _permissions.Add(new Permission(permissionId, Id, userId, access));
            return;
        }

        existing.UpdateAccess(access);
    }

    public void RemovePermission(Guid userId, UserRole userRole)
    {
        if (userRole == UserRole.Administrator)
        {
            throw new DomainException("Los permisos del administrador no pueden ser eliminados.");
        }

        var permission = _permissions.FirstOrDefault(p => p.UserId == userId);
        if (permission != null)
        {
            _permissions.Remove(permission);
        }
    }
}
