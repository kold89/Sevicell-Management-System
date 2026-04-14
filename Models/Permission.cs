using System;
using System.Collections.Generic;

namespace WpfApp1.Models;

public partial class Permission
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string Code { get; set; } = null!;

    public string Modulo { get; set; } = null!;

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
