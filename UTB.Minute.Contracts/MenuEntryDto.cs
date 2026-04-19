namespace UTB.Minute.Contracts;

public record MenuEntryDto(
    int Id,
    DateTime Date,
    MealDto Food,
    int AvailablePortions
);
