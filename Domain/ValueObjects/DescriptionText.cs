using DocumentManagement.Domain.Common;

namespace DocumentManagement.Domain.ValueObjects;

public sealed record DescriptionText
{
    public string Value { get; }

    public DescriptionText(string value)
    {
        if (value.Length > 500)
        {
            throw new DomainException("La descripción no puede superar los 500 caracteres.");
        }

        Value = value.Trim();
    }

    public override string ToString() => Value;
}
