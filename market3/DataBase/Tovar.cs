using System;
using System.Collections.Generic;

namespace market3.DataBase;

public partial class Tovar
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public byte[]? Image { get; set; }

    public int? CategoryId { get; set; }

    public decimal? Price { get; set; }

    public int? Quantity { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<TovarInZakaz> TovarInZakazs { get; } = new List<TovarInZakaz>();
}
