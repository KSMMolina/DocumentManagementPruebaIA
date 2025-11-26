using DocumentManagement.Domain.Common;

namespace DocumentManagement.Domain.ValueObjects;

public sealed record FileSize
{
    public const long MaximumBytes = 50 * 1024 * 1024;
    public long Value { get; }

    public FileSize(long value)
    {
        if (value <= 0)
        {
            throw new DomainException("El tamaño del archivo debe ser mayor a cero.");
        }

        if (value > MaximumBytes)
        {
            throw new DomainException("El tamaño del archivo excede los 50MB permitidos.");
        }

        Value = value;
    }
}
