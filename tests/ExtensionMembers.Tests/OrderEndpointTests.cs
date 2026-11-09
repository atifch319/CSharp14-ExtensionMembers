using System.Net;
using System.Net.Http.Json;
using ExtensionMembers.Api;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ExtensionMembers.Tests;

public class OrderEndpointTests
{
    [Fact]
    public async Task Get_all_returns_the_seeded_orders_with_computed_fields()
    {
        await using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var orders = await client.GetFromJsonAsync<List<OrderSummary>>("/api/orders");

        Assert.NotNull(orders);
        Assert.Collection(
            orders,
            northwind =>
            {
                Assert.Equal(1, northwind.Id);
                Assert.Equal("Northwind Traders", northwind.CustomerName);
                Assert.Equal(1250.00m, northwind.TotalAmount);
                Assert.Equal("Submitted", northwind.Status);
                Assert.True(northwind.IsHighValue);
                Assert.Equal("High", northwind.Priority);
                Assert.True(northwind.CanCancel);
            },
            contoso =>
            {
                Assert.Equal("Contoso", contoso.CustomerName);
                Assert.Equal(75.50m, contoso.TotalAmount);
                Assert.Equal("Draft", contoso.Status);
                Assert.False(contoso.IsHighValue);
                Assert.Equal("Standard", contoso.Priority);
                Assert.True(contoso.CanCancel);
            },
            fabrikam =>
            {
                Assert.Equal("Fabrikam", fabrikam.CustomerName);
                Assert.Equal(2400.00m, fabrikam.TotalAmount);
                Assert.Equal("Shipped", fabrikam.Status);
                Assert.True(fabrikam.IsHighValue);
                Assert.False(fabrikam.CanCancel);
            });
    }

    [Fact]
    public async Task Get_by_id_returns_one_order_or_not_found()
    {
        await using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var order = await client.GetFromJsonAsync<OrderSummary>("/api/orders/2");
        var missing = await client.GetAsync("/api/orders/99");

        Assert.NotNull(order);
        Assert.Equal("Contoso", order.CustomerName);
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
    }

    [Fact]
    public async Task High_value_filter_returns_only_orders_at_or_above_the_threshold()
    {
        await using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var orders = await client.GetFromJsonAsync<List<OrderSummary>>("/api/orders/high-value");

        Assert.NotNull(orders);
        Assert.Equal(["Northwind Traders", "Fabrikam"], orders.Select(order => order.CustomerName));
        Assert.All(orders, order => Assert.True(order.IsHighValue));
    }

    [Fact]
    public async Task Insights_reports_the_threshold_counts_and_empty_state()
    {
        await using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var insights = await client.GetFromJsonAsync<OrderInsights>("/api/orders/insights");

        Assert.NotNull(insights);
        Assert.Equal(1000m, insights.HighValueThreshold);
        Assert.Equal(3, insights.TotalOrders);
        Assert.Equal(2, insights.HighValueOrders);
        Assert.False(insights.IsEmpty);
    }

    [Fact]
    public async Task Post_places_a_submitted_order_and_computes_its_rules()
    {
        await using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/orders", new
        {
            customerName = "Ada Lovelace",
            totalAmount = 1500m
        });
        var created = await response.Content.ReadFromJsonAsync<OrderSummary>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.EndsWith("/api/orders/4", response.Headers.Location.AbsolutePath);
        Assert.NotNull(created);
        Assert.Equal(4, created.Id);
        Assert.Equal("Ada Lovelace", created.CustomerName);
        Assert.Equal(1500m, created.TotalAmount);
        Assert.Equal("Submitted", created.Status);
        Assert.True(created.IsHighValue);
        Assert.Equal("High", created.Priority);
        Assert.True(created.CanCancel);
    }

    [Fact]
    public async Task Post_rejects_a_missing_name_and_a_negative_amount()
    {
        await using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var missingName = await client.PostAsJsonAsync("/api/orders", new { totalAmount = 25m });
        var negative = await client.PostAsJsonAsync("/api/orders", new
        {
            customerName = "Contoso",
            totalAmount = -1m
        });

        Assert.Equal(HttpStatusCode.BadRequest, missingName.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, negative.StatusCode);
        Assert.Contains("Customer name is required.", await missingName.Content.ReadAsStringAsync());
        Assert.Contains("Total amount cannot be negative.", await negative.Content.ReadAsStringAsync());
    }
}
