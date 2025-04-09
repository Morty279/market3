using System;
using System.Collections.Generic;

namespace market3.DataBase;

public partial class TovarInZakaz
{
    public int Id { get; set; }

    public int? ZakazId { get; set; }

    public int? TovarId { get; set; }

    public int? Quantity { get; set; }

    public decimal? Price { get; set; }

    public virtual Tovar? Tovar { get; set; }

    public virtual Zakaz? Zakaz { get; set; }
}
