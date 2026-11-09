namespace ExtensionMembers.Api;

public enum OrderStatus
{
    Draft,
    Submitted,
    Shipped,
    Cancelled
}

public sealed class Order
{
    public int Id { get; init; }

    public required string CustomerName { get; init; }

    public decimal TotalAmount { get; init; }

    public OrderStatus Status { get; init; }

    public DateTimeOffset CreatedAt { get; init; }
}

public sealed class CreateOrderRequest
{
    public string? CustomerName { get; set; }

    public decimal TotalAmount { get; set; }
}

public sealed record OrderSummary(
    int Id,
    string CustomerName,
    decimal TotalAmount,
    string Status,
    bool IsHighValue,
    string Priority,
    bool CanCancel,
    DateTimeOffset CreatedAt);

public sealed record OrderInsights(
    decimal HighValueThreshold,
    int TotalOrders,
    int HighValueOrders,
    bool IsEmpty);
