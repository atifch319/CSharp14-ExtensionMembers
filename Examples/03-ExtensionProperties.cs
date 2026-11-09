/*
Explanation: An extension property is computed when it is read. It is not a field on the object.
Before: order.IsHighValue() is a method, so the call needs parentheses.
After: order.IsHighValue reads like data. ReviewNote forwards get and set to the Note the object already stores.
Usage: ExtensionPropertyDemo compares the method and the property, then trims a review note.
Expected: 1000 and above is high value. Assigning "  rush  " stores "rush" in Note.
When to use: The member is a fact about the receiver, the getter has no side effects, and the name reads like data.
Limitations: No backing field, no init accessor, and no auto-property. A property does not override an instance member. A method and a property of the same name are ambiguous (CS9339), which is why this file uses two types. Expression trees cannot contain the property (CS9296), so IQueryable cannot use it. Enumerable.Where can.
*/

namespace ExtensionMembers.Examples.Properties;

// BEFORE — behavior expressed as a method.
public sealed class MethodOrder
{
    public decimal TotalAmount { get; init; }
}

public static class MethodOrderExtensions
{
    public static bool IsHighValue(this MethodOrder order) => order.TotalAmount >= 1000m;
}

// AFTER — behavior expressed as properties. ReviewNote writes through to Note.
public sealed class PropertyOrder
{
    public decimal TotalAmount { get; init; }

    public string Note { get; set; } = "";
}

public static class PropertyOrderExtensions
{
    extension(PropertyOrder order)
    {
        public bool IsHighValue => order.TotalAmount >= 1000m;

        public string ReviewNote
        {
            get => order.Note;
            set => order.Note = value.Trim();
        }
    }
}

// USAGE
public static class ExtensionPropertyDemo
{
    public static bool MethodIsHighValue(decimal totalAmount) =>
        new MethodOrder { TotalAmount = totalAmount }.IsHighValue();

    public static bool PropertyIsHighValue(decimal totalAmount) =>
        new PropertyOrder { TotalAmount = totalAmount }.IsHighValue;

    public static (string ThroughExtension, string StoredNote) TrimReviewNote(string note)
    {
        var order = new PropertyOrder();
        order.ReviewNote = note;
        return (order.ReviewNote, order.Note);
    }
}
