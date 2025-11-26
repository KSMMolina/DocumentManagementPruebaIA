namespace DocumentManagement.Domain.Enums;

public enum AuditAction
{
    FolderCreated,
    FolderRenamed,
    FolderDeleted,
    FileUploaded,
    FileUpdated,
    FileDeleted,
    FileDownloaded,
    PermissionGranted,
    PermissionRevoked
}
