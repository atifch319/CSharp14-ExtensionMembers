/*
Explanation: An extension block groups members for one receiver inside a top-level non-generic static class.
Before: Each behavior is its own static method with a this parameter.
After: The same methods sit together in extension(BlockOrder order). Callers still use parentheses.
Usage: ExtensionBlockDemo runs the classic methods and the block methods.
Expected: Draft and Submitted can be cancelled. Shipped and Cancelled cannot. 1000 and above is high value.
When to use: Several related methods belong together and you do not want to change any call site.
Limitations: Moving a method into a block is source compatible, but it still adds no fields. Two members in the same static class cannot share a signature. This file uses two order types so both forms can compile together.
*/

namespace ExtensionMembers.Examples.Blocks;

public enum OrderStatus
{
    Draft,
    Submitted,
    Shipped,
    Cancelled
}

// BEFORE — separate extension methods.
public sealed class ClassicOrder
{
    public decimal TotalAmount { get; init; }

    public OrderStatus Status { get; init; }
}

public static class ClassicOrderExtensions
{
    public static bool IsHighValue(this ClassicOrder order) => order.TotalAmount >= 1000m;

    public static bool CanCancel(this ClassicOrder order) =>
        order.Status is OrderStatus.Draft or OrderStatus.Submitted;
}

// AFTER — the same methods, grouped in one block.
public sealed class BlockOrder
{
    public decimal TotalAmount { get; init; }

    public OrderStatus Status { get; init; }
}

public static class BlockOrderExtensions
{
    extension(BlockOrder order)
    {
        public bool IsHighValue() => order.TotalAmount >= 1000m;

        public bool CanCancel() => order.Status is OrderStatus.Draft or OrderStatus.Submitted;
    }
}

// USAGE
public static class ExtensionBlockDemo
{
    public static bool ClassicIsHighValue(decimal totalAmount) =>
        new ClassicOrder { TotalAmount = totalAmount }.IsHighValue();

    public static bool BlockIsHighValue(decimal totalAmount) =>
        new BlockOrder { TotalAmount = totalAmount }.IsHighValue();

    public static bool ClassicCanCancel(OrderStatus status) =>
        new ClassicOrder { Status = status }.CanCancel();

    public static bool BlockCanCancel(OrderStatus status) =>
        new BlockOrder { Status = status }.CanCancel();
}
