namespace Order.Domain.Enums;

public enum OrderStatus
{
    Created = 0,
    InProgress = 1,
    Shipped = 2,
    Completed = 3,
    Cancelled = 4
}