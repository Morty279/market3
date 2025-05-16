using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

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
    [JsonIgnore]

    public virtual ICollection<TovarInZakaz> TovarInZakazs { get; } = new List<TovarInZakaz>();
}

public class TovarDTO
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public byte[]? Image { get; set; }

    public int? CategoryId { get; set; }

    public decimal? Price { get; set; }

    public int? Quantity { get; set; }

    public string? CategoryName { get; set; }

    public static explicit operator TovarDTO(Tovar tovar)
    {
        TovarDTO result = new TovarDTO
        {
            CategoryId = tovar.CategoryId,
            CategoryName = tovar.Category?.Name,
            Description = tovar.Description,
            Id = tovar.Id,
            Image = tovar.Image,
            Name = tovar.Name,
            Price = tovar.Price,
            Quantity = tovar.Quantity


        };
        return result;
    }

}