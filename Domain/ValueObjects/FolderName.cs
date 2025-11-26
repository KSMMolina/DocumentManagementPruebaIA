using DocumentManagement.Domain.Common;

namespace DocumentManagement.Domain.ValueObjects;

public sealed record FolderName
{
    public string Value { get; }

    public FolderName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException("El nombre de la carpeta es obligatorio.");
        }

        if (value.Length > 100)
        {
            throw new DomainException("El nombre de la carpeta no puede superar los 100 caracteres.");
        }

        Value = value.Trim();
    }

    public override string ToString() => Value;
}
