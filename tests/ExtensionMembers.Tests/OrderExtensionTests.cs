using ExtensionMembers.Api;

namespace ExtensionMembers.Tests;

public class OrderExtensionTests
{
    [Theory]
    [InlineData("999.99", false, "Standard")]
    [InlineData("1000", true, "High")]
    [InlineData("1250.00", true, "High")]
    public void High_value_uses_the_shared_threshold(string amount, bool isHighValue, string priority)
    {
        var order = Sample(decimal.Parse(amount, System.Globalization.CultureInfo.InvariantCulture), OrderStatus.Submitted);

        Assert.Equal(1000m, Order.HighValueThreshold);
        Assert.Equal(isHighValue, order.IsHighValue);
        Assert.Equal(priority, order.Priority);
    }

    [Theory]
    [InlineData(OrderStatus.Draft, true)]
    [InlineData(OrderStatus.Submitted, true)]
    [InlineData(OrderStatus.Shipped, false)]
    [InlineData(OrderStatus.Cancelled, false)]
    public void Only_draft_and_submitted_orders_can_be_cancelled(OrderStatus status, bool canCancel)
    {
        var order = Sample(1000m, status);

        Assert.Equal(canCancel, order.CanCancel);
    }

    [Fact]
    public void Summary_copies_stored_data_and_computed_rules()
    {
        var createdAt = new DateTimeOffset(2026, 3, 1, 9, 0, 0, TimeSpan.Zero);
        var order = new Order
        {
            Id = 7,
            CustomerName = "Northwind Traders",
            TotalAmount = 1250m,
            Status = OrderStatus.Submitted,
            CreatedAt = createdAt
        };

        var summary = order.ToSummary();

        Assert.Equal(7, summary.Id);
        Assert.Equal("Northwind Traders", summary.CustomerName);
        Assert.Equal(1250m, summary.TotalAmount);
        Assert.Equal("Submitted", summary.Status);
        Assert.True(summary.IsHighValue);
        Assert.Equal("High", summary.Priority);
        Assert.True(summary.CanCancel);
        Assert.Equal(createdAt, summary.CreatedAt);
    }

    [Fact]
    public void Empty_check_uses_the_generic_extension()
    {
        IReadOnlyCollection<Order> none = [];
        IReadOnlyCollection<Order> orders = [Sample(75.50m, OrderStatus.Draft)];

        Assert.True(none.IsEmpty);
        Assert.False(orders.IsEmpty);
    }

    private static Order Sample(decimal totalAmount, OrderStatus status) => new()
    {
        Id = 1,
        CustomerName = "Contoso",
        TotalAmount = totalAmount,
        Status = status,
        CreatedAt = DateTimeOffset.UnixEpoch
    };
}
