using DocumentManagement.Domain.Common;
using DocumentManagement.Domain.Enums;

namespace DocumentManagement.Domain.Entities;

public sealed class AuditEntry : Entity<Guid>
{
    public Guid PropertyId { get; }
    public Guid? FolderId { get; }
    public Guid? FileId { get; }
    public Guid ActorId { get; }
    public AuditAction Action { get; }
    public DateTime OccurredAt { get; }
    public string Details { get; }

    private AuditEntry() : base(Guid.Empty)
    {
        PropertyId = Guid.Empty;
        ActorId = Guid.Empty;
        Action = AuditAction.FolderCreated;
        OccurredAt = DateTime.MinValue;
        Details = string.Empty;
    }

    public AuditEntry(Guid id, Guid propertyId, Guid? folderId, Guid? fileId, Guid actorId, AuditAction action, DateTime occurredAt, string details) : base(id)
    {
        PropertyId = propertyId;
        FolderId = folderId;
        FileId = fileId;
        ActorId = actorId;
        Action = action;
        OccurredAt = occurredAt;
        Details = details;
    }
}
