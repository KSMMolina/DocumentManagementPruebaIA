using DocumentManagement.Domain.Common;
using DocumentManagement.Domain.ValueObjects;

namespace DocumentManagement.Domain.Entities;

public sealed class DocumentFile : Entity<Guid>
{
    public Guid FolderId { get; private set; }
    public FileName Name { get; private set; }
    public DescriptionText Description { get; private set; }
    public FileSize Size { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private DocumentFile() : base(Guid.Empty)
    {
        FolderId = Guid.Empty;
        Name = new FileName("?");
        Description = new DescriptionText(string.Empty);
        Size = new FileSize(1);
        CreatedAt = DateTime.MinValue;
    }

    public DocumentFile(Guid id, Guid folderId, FileName name, DescriptionText description, FileSize size, DateTime createdAt) : base(id)
    {
        FolderId = folderId;
        Name = name;
        Description = description;
        Size = size;
        CreatedAt = createdAt;
    }

    public void UpdateMetadata(FileName newName, DescriptionText newDescription)
    {
        Name = newName;
        Description = newDescription;
    }
}
