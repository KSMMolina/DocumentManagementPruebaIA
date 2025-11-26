using DocumentManagement.Domain.Entities;

namespace DocumentManagement.Domain.Events;

public sealed record PermissionChanged(Permission Permission);
