using DocumentManagement.Domain.Common;

namespace DocumentManagement.Domain.ValueObjects;

public sealed record FolderDepth
{
    public const int MaxDepth = 3;
    public int Value { get; }

    public FolderDepth(int value)
    {
        if (value < 1)
        {
            throw new DomainException("La profundidad debe ser al menos 1.");
        }

        if (value > MaxDepth)
        {
            throw new DomainException("No se permiten niveles de carpetas superiores a 3.");
        }

        Value = value;
    }
}
