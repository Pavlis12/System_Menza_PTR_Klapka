namespace UTB.Minute.Contracts;

public record MealDto(
    int Id,
    string Description,
    decimal Price,
    bool IsActive
);
