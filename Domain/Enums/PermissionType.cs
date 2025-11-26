namespace DocumentManagement.Domain.Enums;

[Flags]
public enum PermissionType
{
    None = 0,
    View = 1,
    Download = 2
}
