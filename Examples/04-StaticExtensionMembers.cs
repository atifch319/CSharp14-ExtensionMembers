/*
Explanation: A static extension member is called on the type, as in Order.HighValueThreshold.
Before: OrderRules owns the threshold and the draft factory, so callers must know that helper's name.
After: extension(Order) puts those members on Order itself when this namespace is in scope.
Usage: StaticExtensionDemo reads the threshold and creates a draft both ways.
Expected: Both thresholds are 1000. Both factories return a Draft order for the customer name you pass.
When to use: The behavior belongs to the type rather than to one instance, such as a shared threshold or a factory.
Limitations: The extension is visible only when its static class is in scope. A static member declared on Order itself wins. Reflection on Order does not list the extension. An extension block with no receiver name cannot contain instance members.
*/

namespace ExtensionMembers.Examples.StaticMembers;

public enum OrderStatus
{
    Draft,
    Submitted,
    Shipped,
    Cancelled
}

public sealed class Order
{
    public string CustomerName { get; init; } = "";

    public decimal TotalAmount { get; init; }

    public OrderStatus Status { get; init; }
}

// BEFORE — a separate static class.
public static class OrderRules
{
    public static decimal HighValueThreshold => 1000m;

    public static Order CreateDraft(string customerName) => new()
    {
        CustomerName = customerName,
        Status = OrderStatus.Draft
    };
}

// AFTER — static members of the extended type.
public static class OrderStaticExtensions
{
    extension(Order)
    {
        public static decimal HighValueThreshold => 1000m;

        public static Order CreateDraft(string customerName) => new()
        {
            CustomerName = customerName,
            Status = OrderStatus.Draft
        };
    }
}

// USAGE
public static class StaticExtensionDemo
{
    public static decimal HelperThreshold() => OrderRules.HighValueThreshold;

    public static decimal ExtensionThreshold() => Order.HighValueThreshold;

    public static Order HelperDraft(string customerName) => OrderRules.CreateDraft(customerName);

    public static Order ExtensionDraft(string customerName) => Order.CreateDraft(customerName);
}
