using System.ComponentModel.DataAnnotations;
using UTB.Minute.Contracts;

namespace UTB.Minute.Db.Entities
{
    internal class Order
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public MenuEntry MenuEntry { get; set; }

        public OrderStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
