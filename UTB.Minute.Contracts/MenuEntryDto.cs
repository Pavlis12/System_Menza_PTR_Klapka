namespace UTB.Minute.Contracts;

public record MenuEntryDto(
    int Id,
    DateTime Date,
    FoodDto Food,
    int PortionsAvailable
);
