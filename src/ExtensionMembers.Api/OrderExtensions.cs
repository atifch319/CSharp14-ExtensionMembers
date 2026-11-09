namespace ExtensionMembers.Api;

public static class OrderExtensions
{
    extension(Order order)
    {
        public bool IsHighValue => order.TotalAmount >= Order.HighValueThreshold;

        public string Priority => order.IsHighValue ? "High" : "Standard";

        public bool CanCancel => order.Status is OrderStatus.Draft or OrderStatus.Submitted;

        public OrderSummary ToSummary() => new(
            order.Id,
            order.CustomerName,
            order.TotalAmount,
            order.Status.ToString(),
            order.IsHighValue,
            order.Priority,
            order.CanCancel,
            order.CreatedAt);
    }

    extension(Order)
    {
        public static decimal HighValueThreshold => 1000m;
    }
}
