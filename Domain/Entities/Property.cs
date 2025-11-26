using DocumentManagement.Domain.Common;
using DocumentManagement.Domain.ValueObjects;

namespace DocumentManagement.Domain.Entities;

public sealed class Property : Entity<Guid>
{
    public string Name { get; private set; }

    private Property() : base(Guid.Empty)
    {
        Name = string.Empty;
    }

    public Property(Guid id, string name) : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("El nombre de la propiedad es obligatorio.");
        }

        Name = name.Trim();
    }
}
