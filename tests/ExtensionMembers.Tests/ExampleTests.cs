using System.Globalization;
using ExtensionMembers.Examples.Blocks;
using ExtensionMembers.Examples.Comparison;
using ExtensionMembers.Examples.Generic;
using ExtensionMembers.Examples.Properties;
using ExtensionMembers.Examples.StaticMembers;
using ExtensionMembers.Examples.Traditional;
using BlockOrderStatus = ExtensionMembers.Examples.Blocks.OrderStatus;
using ComparisonOrderStatus = ExtensionMembers.Examples.Comparison.OrderStatus;
using StaticOrderStatus = ExtensionMembers.Examples.StaticMembers.OrderStatus;

namespace ExtensionMembers.Tests;

public class ExampleTests
{
    [Theory]
    [InlineData("999.99", false)]
    [InlineData("1000", true)]
    [InlineData("1250.00", true)]
    public void Traditional_helper_and_extension_agree(string amount, bool expected)
    {
        var total = Parse(amount);

        Assert.Equal(expected, TraditionalExtensionDemo.HelperIsHighValue(total));
        Assert.Equal(expected, TraditionalExtensionDemo.ExtensionIsHighValue(total));
    }

    [Theory]
    [InlineData("999.99", false)]
    [InlineData("1000", true)]
    public void Extension_block_methods_match_classic_extension_methods(string amount, bool expected)
    {
        var total = Parse(amount);

        Assert.Equal(expected, ExtensionBlockDemo.ClassicIsHighValue(total));
        Assert.Equal(expected, ExtensionBlockDemo.BlockIsHighValue(total));
    }

    [Theory]
    [InlineData(BlockOrderStatus.Draft, true)]
    [InlineData(BlockOrderStatus.Submitted, true)]
    [InlineData(BlockOrderStatus.Shipped, false)]
    [InlineData(BlockOrderStatus.Cancelled, false)]
    public void Extension_block_cancellation_matches_the_classic_method(BlockOrderStatus status, bool expected)
    {
        Assert.Equal(expected, ExtensionBlockDemo.ClassicCanCancel(status));
        Assert.Equal(expected, ExtensionBlockDemo.BlockCanCancel(status));
    }

    [Theory]
    [InlineData("999.99", false)]
    [InlineData("1000", true)]
    public void Extension_property_matches_the_extension_method(string amount, bool expected)
    {
        var total = Parse(amount);

        Assert.Equal(expected, ExtensionPropertyDemo.MethodIsHighValue(total));
        Assert.Equal(expected, ExtensionPropertyDemo.PropertyIsHighValue(total));
    }

    [Fact]
    public void Extension_property_setter_updates_the_existing_note()
    {
        var (throughExtension, storedNote) = ExtensionPropertyDemo.TrimReviewNote("  rush  ");

        Assert.Equal("rush", throughExtension);
        Assert.Equal("rush", storedNote);
    }

    [Fact]
    public void Static_extension_members_match_the_helper()
    {
        Assert.Equal(1000m, StaticExtensionDemo.HelperThreshold());
        Assert.Equal(StaticExtensionDemo.HelperThreshold(), StaticExtensionDemo.ExtensionThreshold());

        var helperDraft = StaticExtensionDemo.HelperDraft("Ada Lovelace");
        var extensionDraft = StaticExtensionDemo.ExtensionDraft("Ada Lovelace");

        Assert.Equal(helperDraft.CustomerName, extensionDraft.CustomerName);
        Assert.Equal(StaticOrderStatus.Draft, helperDraft.Status);
        Assert.Equal(StaticOrderStatus.Draft, extensionDraft.Status);
    }

    [Fact]
    public void Generic_extension_members_match_the_helpers()
    {
        IReadOnlyCollection<decimal> totals = [1250m, 75.50m];
        IReadOnlyCollection<decimal> none = [];

        Assert.False(GenericExtensionDemo.HelperIsEmpty(totals));
        Assert.False(GenericExtensionDemo.ExtensionIsEmpty(totals));
        Assert.True(GenericExtensionDemo.HelperIsEmpty(none));
        Assert.True(GenericExtensionDemo.ExtensionIsEmpty(none));
        Assert.True(GenericExtensionDemo.HelperHasItems(totals));
        Assert.True(GenericExtensionDemo.ExtensionHasItems(totals));
        Assert.False(GenericExtensionDemo.ExtensionHasItems(none));
        Assert.False(GenericExtensionDemo.ExtensionHasItems(null));
    }

    [Theory]
    [InlineData("75.50", ComparisonOrderStatus.Draft, false, true, "Standard")]
    [InlineData("1000", ComparisonOrderStatus.Submitted, true, true, "High")]
    [InlineData("2400", ComparisonOrderStatus.Shipped, true, false, "High")]
    [InlineData("0", ComparisonOrderStatus.Cancelled, false, false, "Standard")]
    public void Before_and_after_rules_agree(
        string amount,
        ComparisonOrderStatus status,
        bool isHighValue,
        bool canCancel,
        string priority)
    {
        var (before, after) = BeforeAfterDemo.Compare(Parse(amount), status);

        Assert.Equal(before, after);
        Assert.Equal(isHighValue, after.IsHighValue);
        Assert.Equal(canCancel, after.CanCancel);
        Assert.Equal(priority, after.Priority);
        Assert.Equal(1000m, after.Threshold);
    }

    [Fact]
    public void Generic_empty_check_detects_an_empty_order_list()
    {
        Assert.True(BeforeAfterDemo.EmptyOrdersDetected());
        Assert.False(BeforeAfterDemo.PopulatedOrdersDetected());
    }

    private static decimal Parse(string amount) =>
        decimal.Parse(amount, CultureInfo.InvariantCulture);
}
