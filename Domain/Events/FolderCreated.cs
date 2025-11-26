using DocumentManagement.Domain.Entities;

namespace DocumentManagement.Domain.Events;

public sealed record FolderCreated(Folder Folder);
