using DocumentManagement.Domain.Enums;

namespace DocumentManagement.Application.DTOs;

public record PermissionRequest(Guid FolderId, Guid UserId, PermissionType Access, UserRole UserRole);
