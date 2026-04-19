using System.ComponentModel.DataAnnotations;

namespace UTB.Minute.Db.Entities;

public class MenuEntry
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public int PortionsAvailable { get; set; }

    // Cizí klíč k jídlu
    public int FoodId { get; set; }
    public virtual Food Food { get; set; } = null!;
}
