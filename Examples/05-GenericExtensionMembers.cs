/*
Explanation: The type parameter goes on the extension block when the receiver uses it.
Before: CollectionCalculations.IsEmpty(totals) and CollectionCalculations.HasItems(totals) are generic helpers.
After: totals.IsEmpty is an instance member, and IReadOnlyCollection<decimal>.HasItems(totals) is a static member.
Usage: GenericExtensionDemo checks an empty list, a list of order totals, and a null list.
Expected: An empty collection reports empty. A collection with one total does not. HasItems is false for null and for an empty collection.
When to use: The same rule applies to every element type, including orders, totals, or any other list you already have.
Limitations: Put receiver type parameters on the extension declaration, not again on the member. A null receiver is not safe unless the member checks it. The static HasItems method can accept null. The instance IsEmpty property cannot.
*/

namespace ExtensionMembers.Examples.Generic;

// BEFORE — generic helpers.
public static class CollectionCalculations
{
    public static bool IsEmpty<T>(IReadOnlyCollection<T> items) => items.Count == 0;

    public static bool HasItems<T>(IReadOnlyCollection<T>? items) => items is { Count: > 0 };
}

// AFTER — one instance block and one static block.
public static class CollectionExtensions
{
    extension<T>(IReadOnlyCollection<T> items)
    {
        public bool IsEmpty => items.Count == 0;
    }

    extension<T>(IReadOnlyCollection<T>)
    {
        public static bool HasItems(IReadOnlyCollection<T>? items) => items is { Count: > 0 };
    }
}

// USAGE
public static class GenericExtensionDemo
{
    public static bool HelperIsEmpty(IReadOnlyCollection<decimal> totals) =>
        CollectionCalculations.IsEmpty(totals);

    public static bool ExtensionIsEmpty(IReadOnlyCollection<decimal> totals) => totals.IsEmpty;

    public static bool HelperHasItems(IReadOnlyCollection<decimal>? totals) =>
        CollectionCalculations.HasItems(totals);

    public static bool ExtensionHasItems(IReadOnlyCollection<decimal>? totals) =>
        IReadOnlyCollection<decimal>.HasItems(totals);
}
