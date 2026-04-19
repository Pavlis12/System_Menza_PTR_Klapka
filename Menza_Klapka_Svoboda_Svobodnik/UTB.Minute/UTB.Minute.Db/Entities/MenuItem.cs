using System;
using System.Collections.Generic;
using System.Text;

namespace UTB.Minute.Db.Entities;

public class MenuItem
{
    public Guid Id { get; set; }
    public Guid MealId { get; set; }
    public DateTime Date { get; set; }
    public int AvailablePortions { get; set; }
    public Meal? Meal { get; set; }
    public ICollection<Order> Orders { get; set; } = [];
}
