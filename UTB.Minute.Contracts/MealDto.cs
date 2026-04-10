namespace UTB.Minute.Contracts;

public record MealDto(
    int Id,
    string Name,
    decimal Price,
    bool IsActive
);
