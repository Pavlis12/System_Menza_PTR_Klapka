using System;
using System.Collections.Generic;
using System.Text;

namespace UTB.Minute.Contracts.Dtos
{
    public record MenuItemResponseDto(Guid Id, Guid MealId, string MealName, DateTime Date, int AvailablePortions);
    public record UpdateMenuItemRequest(DateTime Date, int AvailablePortions);
    public record CreateMenuItemRequest(Guid MealId, DateTime Date, int AvailablePortions);
}
