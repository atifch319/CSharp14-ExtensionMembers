/*
Explanation: A traditional extension method is a static method whose first parameter uses this.
Before: OrderCalculations.IsHighValue(order) keeps the rule in a helper. The order is an ordinary argument.
After: order.IsHighValue() is the same rule, discovered on the instance when this namespace is in scope.
Usage: TraditionalExtensionDemo calls both forms with the same amount.
Expected: 1000 and above is high value. 999.99 is not.
When to use: You only need a method, and callers can use a compiler older than C# 14.
Limitations: This form has no properties and no static members of Order. It adds no stored state and does not override a method the type already declares.
*/

namespace ExtensionMembers.Examples.Traditional;

// BEFORE — the helper does not appear in IntelliSense on the order.
public static class OrderCalculations
{
    public static bool IsHighValue(Order order) => order.TotalAmount >= 1000m;
}

// AFTER — the classic extension method.
public static class TraditionalOrderExtensions
{
    public static bool IsHighValue(this Order order) => order.TotalAmount >= 1000m;
}

public sealed class Order
{
    public decimal TotalAmount { get; init; }
}

// USAGE
public static class TraditionalExtensionDemo
{
    public static bool HelperIsHighValue(decimal totalAmount)
    {
        var order = new Order { TotalAmount = totalAmount };
        return OrderCalculations.IsHighValue(order);
    }

    public static bool ExtensionIsHighValue(decimal totalAmount)
    {
        var order = new Order { TotalAmount = totalAmount };
        return order.IsHighValue();
    }
}
