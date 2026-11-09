/*
Explanation: The same order rules can live in a helper or in extension members. The results match.
Before: OrderRules.IsHighValue(order), OrderRules.Priority(order), and OrderRules.HighValueThreshold.
After: order.IsHighValue, order.Priority, order.CanCancel, and Order.HighValueThreshold, plus orders.IsEmpty.
Usage: BeforeAfterDemo.Compare runs both styles on one order. EmptyOrdersDetected checks a list.
Expected: Both styles agree. 1000 and above is High. Draft and Submitted can be cancelled. An empty list is detected.
When to use: Read this file when you want the whole feature in one place, then open the API for the practical version.
Limitations: Extension properties add no stored state and do not override instance members. They are not a performance change. A C# 14 compiler is required for the new syntax. See docs/compatibility.md.
*/

namespace ExtensionMembers.Examples.Comparison;

public enum OrderStatus
{
    Draft,
    Submitted,
    Shipped,
    Cancelled
}

public sealed class Order
{
    public decimal TotalAmount { get; init; }

    public OrderStatus Status { get; init; }
}

public sealed record RuleResult(bool IsHighValue, bool CanCancel, string Priority, decimal Threshold);

// BEFORE — one helper class owns every rule.
public static class OrderRules
{
    public static decimal HighValueThreshold => 1000m;

    public static bool IsHighValue(Order order) => order.TotalAmount >= HighValueThreshold;

    public static bool CanCancel(Order order) =>
        order.Status is OrderStatus.Draft or OrderStatus.Submitted;

    public static string Priority(Order order) => IsHighValue(order) ? "High" : "Standard";
}

// AFTER — instance members, a static threshold, and a generic list check.
public static class OrderFeatureExtensions
{
    extension(Order order)
    {
        public bool IsHighValue => order.TotalAmount >= Order.HighValueThreshold;

        public string Priority => order.IsHighValue ? "High" : "Standard";

        public bool CanCancel => order.Status is OrderStatus.Draft or OrderStatus.Submitted;
    }

    extension(Order)
    {
        public static decimal HighValueThreshold => 1000m;
    }

    extension<T>(IReadOnlyCollection<T> items)
    {
        public bool IsEmpty => items.Count == 0;
    }
}

// USAGE
public static class BeforeAfterDemo
{
    public static (RuleResult Before, RuleResult After) Compare(decimal totalAmount, OrderStatus status)
    {
        var order = new Order { TotalAmount = totalAmount, Status = status };

        var before = new RuleResult(
            OrderRules.IsHighValue(order),
            OrderRules.CanCancel(order),
            OrderRules.Priority(order),
            OrderRules.HighValueThreshold);

        var after = new RuleResult(
            order.IsHighValue,
            order.CanCancel,
            order.Priority,
            Order.HighValueThreshold);

        return (before, after);
    }

    public static bool EmptyOrdersDetected()
    {
        IReadOnlyCollection<Order> orders = [];
        return orders.IsEmpty;
    }

    public static bool PopulatedOrdersDetected()
    {
        IReadOnlyCollection<Order> orders = [new() { TotalAmount = 75.50m, Status = OrderStatus.Draft }];
        return orders.IsEmpty;
    }
}
