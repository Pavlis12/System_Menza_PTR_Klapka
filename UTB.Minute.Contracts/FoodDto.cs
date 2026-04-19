namespace UTB.Minute.Contracts;

public record FoodDto(
    int Id,
    string Description,
    decimal Price,
    bool IsActive
);
