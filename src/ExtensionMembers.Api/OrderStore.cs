namespace ExtensionMembers.Api;

public sealed class OrderStore
{
    private readonly Lock _gate = new();
    private readonly List<Order> _orders = [];
    private int _nextId;

    public OrderStore()
    {
        Add("Northwind Traders", 1250.00m, OrderStatus.Submitted, new DateTimeOffset(2026, 3, 1, 9, 0, 0, TimeSpan.Zero));
        Add("Contoso", 75.50m, OrderStatus.Draft, new DateTimeOffset(2026, 3, 2, 14, 30, 0, TimeSpan.Zero));
        Add("Fabrikam", 2400.00m, OrderStatus.Shipped, new DateTimeOffset(2026, 3, 3, 11, 15, 0, TimeSpan.Zero));
    }

    public IReadOnlyCollection<Order> GetAll()
    {
        lock (_gate)
        {
            return _orders.ToArray();
        }
    }

    public Order? Find(int id)
    {
        lock (_gate)
        {
            return _orders.FirstOrDefault(order => order.Id == id);
        }
    }

    public Order Add(string customerName, decimal totalAmount) =>
        Add(customerName, totalAmount, OrderStatus.Submitted, DateTimeOffset.UtcNow);

    private Order Add(string customerName, decimal totalAmount, OrderStatus status, DateTimeOffset createdAt)
    {
        lock (_gate)
        {
            var order = new Order
            {
                Id = ++_nextId,
                CustomerName = customerName,
                TotalAmount = totalAmount,
                Status = status,
                CreatedAt = createdAt
            };

            _orders.Add(order);
            return order;
        }
    }
}
