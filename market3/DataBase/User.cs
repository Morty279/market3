using System;
using System.Collections.Generic;

namespace market3.DataBase;

public partial class User
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? RoleId { get; set; }

    public string? Password { get; set; }

    public virtual Role? Role { get; set; }

    public virtual ICollection<Zakaz> Zakazs { get; } = new List<Zakaz>();
}
