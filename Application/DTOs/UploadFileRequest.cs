namespace DocumentManagement.Application.DTOs;

public record UploadFileRequest(Guid FolderId, string Name, string Description, long SizeInBytes);
