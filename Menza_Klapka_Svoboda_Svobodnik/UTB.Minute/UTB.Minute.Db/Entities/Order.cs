using System;
using System.Collections.Generic;
using System.Text;
using UTB.Minute.Contracts;

namespace UTB.Minute.Db.Entities;

public class Order
{
    public Guid Id { get; set; }
    public Guid MenuItemId { get; set; }
    public int OrderNumber { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Preparing;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public MenuItem? MenuItem { get; set; }
}