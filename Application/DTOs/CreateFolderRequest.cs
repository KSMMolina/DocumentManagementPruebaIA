namespace DocumentManagement.Application.DTOs;

public record CreateFolderRequest(Guid PropertyId, Guid? ParentFolderId, string Name);
