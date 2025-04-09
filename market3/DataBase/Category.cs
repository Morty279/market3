using System;
using System.Collections.Generic;

namespace market3.DataBase;

public partial class Category
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Tovar> Tovars { get; } = new List<Tovar>();
}
