using DocumentManagement.Domain.Entities;

namespace DocumentManagement.Domain.Events;

public sealed record FileUploaded(DocumentFile File);
