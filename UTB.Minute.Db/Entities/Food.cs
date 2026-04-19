using System.ComponentModel.DataAnnotations;

namespace UTB.Minute.Db.Entities;

public class Food
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public bool IsActive { get; set; } = true;

    // Vazba: Jedno jídlo může být v mnoha položkách menu
    public virtual ICollection<MenuEntry> MenuEntries { get; set; } = new List<MenuEntry>();
}
