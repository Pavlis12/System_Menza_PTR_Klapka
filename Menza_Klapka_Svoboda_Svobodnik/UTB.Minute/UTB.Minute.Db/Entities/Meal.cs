using System;
using System.Collections.Generic;
using System.Text;

namespace UTB.Minute.Db.Entities;

public class Meal
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<MenuItem> MenuItems { get; set; } = [];
}
