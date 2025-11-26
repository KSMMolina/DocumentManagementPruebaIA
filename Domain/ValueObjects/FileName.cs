using DocumentManagement.Domain.Common;

namespace DocumentManagement.Domain.ValueObjects;

public sealed record FileName
{
    public string Value { get; }

    public FileName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException("El nombre del archivo es obligatorio.");
        }

        if (value.Length > 150)
        {
            throw new DomainException("El nombre del archivo no puede superar los 150 caracteres.");
        }

        Value = value.Trim();
    }

    public override string ToString() => Value;
}
