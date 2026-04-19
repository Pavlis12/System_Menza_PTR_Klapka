using System.ComponentModel.DataAnnotations;

namespace UTB.Minute.Db.Entities;

public class MenuEntry
{
    public int Id { get; set; }
    public int MealId { get; set; }
    public Meal Meal { get; set; } = null!;
    public DateTime Date { get; set; }
    public int AvailablePortions { get; set; } // Musí se to jmenovat stejně
}
