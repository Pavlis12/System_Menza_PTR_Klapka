using System;
using System.Collections.Generic;
using System.Text;

namespace UTB.Minute.Contracts.Dtos
{
    public record UpdateMealRequest(string Name, string Description, decimal Price, bool IsActive);
    public record MealResponseDto(Guid Id, string Name, string? Description, decimal Price);
    public record CreateMealRequest(string Name, string? Description, decimal Price);
}
