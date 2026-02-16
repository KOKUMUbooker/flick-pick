namespace FlickPickApp.Models;

public class Role : EntityBase
{
    public RoleEnum RoleValue { get; set; }

    // Navigation property
    public List<User> Users { get; set; } = new List<User>();
}

// When sent as JSON, calling .ToString() returns the "keys" not the values
public enum RoleEnum
{
    Admin = 1,
    User = 2
}

public static class RoleConstants
{
    public static readonly Guid AdminRoleId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    public static readonly Guid UserRoleId = Guid.Parse("00000000-0000-0000-0000-000000000002");
}
