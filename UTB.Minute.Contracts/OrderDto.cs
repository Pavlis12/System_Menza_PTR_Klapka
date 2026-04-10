namespace UTB.Minute.Contracts
{
    public record OrderDto(
    int Id,
    int MenuEntryId,
    string MealDescription,
    OrderStatus Status,
    DateTime CreatedAt
);
}
