namespace UTB.Minute.Db.Entities
{
    public class Order
    {
        public int Id { get; set; } // Tady taky public!
        public int MenuEntryId { get; set; }
        public MenuEntry MenuEntry { get; set; } = null!;
        public UTB.Minute.Contracts.OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
