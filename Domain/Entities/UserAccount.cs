using DocumentManagement.Domain.Common;
using DocumentManagement.Domain.Enums;

namespace DocumentManagement.Domain.Entities;

public sealed class UserAccount : Entity<Guid>
{
    public string FullName { get; private set; }
    public string Email { get; private set; }
    public UserRole Role { get; private set; }

    private UserAccount() : base(Guid.Empty)
    {
        FullName = string.Empty;
        Email = string.Empty;
    }

    public UserAccount(Guid id, string fullName, string email, UserRole role) : base(id)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new DomainException("El nombre del usuario es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new DomainException("El correo electrónico es obligatorio.");
        }

        FullName = fullName.Trim();
        Email = email.Trim().ToLowerInvariant();
        Role = role;
    }
}
