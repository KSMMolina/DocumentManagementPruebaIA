namespace DocumentManagement.Application.DTOs;

public record FolderResponse(Guid Id, Guid PropertyId, Guid? ParentFolderId, string Name, int Depth, int FileCount, int ChildCount);
