using System;
using System.Collections.Generic;

namespace market3.DataBase;

public partial class Zakaz
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public TimeOnly? OrderDate { get; set; }

    public decimal? TotalPrice { get; set; }

    public string? Status { get; set; }

    public string? Address { get; set; }

    public virtual ICollection<TovarInZakaz> TovarInZakazs { get; } = new List<TovarInZakaz>();

    public virtual User? User { get; set; }
}
