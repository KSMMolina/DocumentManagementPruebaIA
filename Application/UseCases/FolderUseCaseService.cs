using DocumentManagement.Application.Abstractions.Persistence;
using DocumentManagement.Application.DTOs;
using DocumentManagement.Domain.Common;
using DocumentManagement.Domain.Entities;
using DocumentManagement.Domain.Enums;
using DocumentManagement.Domain.ValueObjects;

namespace DocumentManagement.Application.UseCases;

public sealed class FolderUseCaseService
{
    private readonly IFolderRepository _folderRepository;
    private readonly IDocumentFileRepository _fileRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IAuditRepository _auditRepository;
    private readonly IUnitOfWork _unitOfWork;

    public FolderUseCaseService(
        IFolderRepository folderRepository,
        IDocumentFileRepository fileRepository,
        IPermissionRepository permissionRepository,
        IAuditRepository auditRepository,
        IUnitOfWork unitOfWork)
    {
        _folderRepository = folderRepository;
        _fileRepository = fileRepository;
        _permissionRepository = permissionRepository;
        _auditRepository = auditRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<FolderResponse> CreateFolderAsync(CreateFolderRequest request, UserAccount actor, CancellationToken cancellationToken = default)
    {
        var folderName = new FolderName(request.Name);
        Folder? parent = null;
        if (request.ParentFolderId.HasValue)
        {
            parent = await _folderRepository.GetByIdAsync(request.ParentFolderId.Value, cancellationToken)
                ?? throw new DomainException("La carpeta padre no existe.");
        }

        var depth = parent is null ? new FolderDepth(1) : new FolderDepth(parent.Depth.Value + 1);
        var folder = new Folder(Guid.NewGuid(), request.PropertyId, folderName, depth, parent?.Id);

        if (parent is not null)
        {
            parent.AddChildFolder(folder.Id, folderName);
            await _folderRepository.UpdateAsync(parent, cancellationToken);
        }

        await _folderRepository.AddAsync(folder, cancellationToken);
        await _auditRepository.AddAsync(new AuditEntry(Guid.NewGuid(), request.PropertyId, folder.Id, null, actor.Id, AuditAction.FolderCreated, DateTime.UtcNow, $"Carpeta {folderName.Value} creada"), cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(folder, 0, 0);
    }

    public async Task<DocumentFile> UploadFileAsync(UploadFileRequest request, UserAccount actor, CancellationToken cancellationToken = default)
    {
        var folder = await _folderRepository.GetByIdAsync(request.FolderId, cancellationToken)
            ?? throw new DomainException("La carpeta destino no existe.");

        var fileCount = await _fileRepository.CountByFolderAsync(folder.Id, cancellationToken);
        if (fileCount >= 5)
        {
            throw new DomainException("La carpeta ya contiene el máximo de 5 archivos.");
        }

        var file = folder.AddFile(Guid.NewGuid(), new FileName(request.Name), new DescriptionText(request.Description), new FileSize(request.SizeInBytes), DateTime.UtcNow);

        await _fileRepository.AddAsync(file, cancellationToken);
        await _auditRepository.AddAsync(new AuditEntry(Guid.NewGuid(), folder.PropertyId, folder.Id, file.Id, actor.Id, AuditAction.FileUploaded, DateTime.UtcNow, $"Archivo {request.Name} agregado"), cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return file;
    }

    public async Task UpdateFileMetadataAsync(Guid fileId, string name, string description, UserAccount actor, CancellationToken cancellationToken = default)
    {
        var file = await _fileRepository.GetByIdAsync(fileId, cancellationToken)
            ?? throw new DomainException("El archivo no existe.");

        var folder = await _folderRepository.GetByIdAsync(file.FolderId, cancellationToken)
            ?? throw new DomainException("La carpeta asociada al archivo no existe.");

        file.UpdateMetadata(new FileName(name), new DescriptionText(description));
        await _fileRepository.UpdateAsync(file, cancellationToken);
        await _auditRepository.AddAsync(new AuditEntry(Guid.NewGuid(), folder.PropertyId, file.FolderId, file.Id, actor.Id, AuditAction.FileUpdated, DateTime.UtcNow, $"Archivo {name} actualizado"), cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task AssignPermissionAsync(PermissionRequest request, CancellationToken cancellationToken = default)
    {
        var folder = await _folderRepository.GetByIdAsync(request.FolderId, cancellationToken)
            ?? throw new DomainException("La carpeta no existe.");

        var existing = await _permissionRepository.GetByFolderAndUserAsync(request.FolderId, request.UserId, cancellationToken);
        if (existing is null)
        {
            var permission = new Permission(Guid.NewGuid(), folder.Id, request.UserId, request.Access);
            folder.AssignPermission(permission.Id, request.UserId, request.Access, request.UserRole);
            await _permissionRepository.AddAsync(permission, cancellationToken);
        }
        else
        {
            folder.AssignPermission(existing.Id, request.UserId, request.Access, request.UserRole);
            await _permissionRepository.UpdateAsync(existing, cancellationToken);
        }

        await _auditRepository.AddAsync(new AuditEntry(Guid.NewGuid(), folder.PropertyId, folder.Id, null, request.UserId, AuditAction.PermissionGranted, DateTime.UtcNow, $"Permisos actualizados: {request.Access}"), cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RemovePermissionAsync(Guid folderId, Guid userId, UserRole role, CancellationToken cancellationToken = default)
    {
        var folder = await _folderRepository.GetByIdAsync(folderId, cancellationToken)
            ?? throw new DomainException("La carpeta no existe.");

        var permission = await _permissionRepository.GetByFolderAndUserAsync(folderId, userId, cancellationToken);
        if (permission is null)
        {
            return;
        }

        folder.RemovePermission(userId, role);
        await _permissionRepository.RemoveAsync(permission.Id, cancellationToken);
        await _auditRepository.AddAsync(new AuditEntry(Guid.NewGuid(), folder.PropertyId, folder.Id, null, userId, AuditAction.PermissionRevoked, DateTime.UtcNow, "Permiso revocado"), cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static FolderResponse Map(Folder folder, int fileCount, int childCount)
    {
        return new FolderResponse(folder.Id, folder.PropertyId, folder.ParentFolderId, folder.Name.Value, folder.Depth.Value, fileCount, childCount);
    }
}
