using System;
using System.Collections.Generic;
using System.Text;

namespace UTB.Minute.Contracts.Dtos
{
    public record OrderResponseDto(Guid Id, int OrderNumber, OrderStatus Status, DateTime CreatedAt, string MealName);
    public record CreateOrderRequest(Guid MenuItemId);
}
