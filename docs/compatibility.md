# Compatibility and limits

C# 14 adds extension blocks. It does not remove traditional extension methods.

This page is the precise version of the limits summarized in the [README](../README.md). The behavior below was checked with the .NET 10 SDK and the C# 14 compiler in this repository.

## What the new syntax adds

An `extension` block groups members for one receiver. It lives in a top-level, non-generic static class.

```csharp
public static class OrderExtensions
{
    extension(Order order)
    {
        public bool IsHighValue => order.TotalAmount >= Order.HighValueThreshold;

        public string Priority => order.IsHighValue ? "High" : "Standard";
    }

    extension(Order)
    {
        public static decimal HighValueThreshold => 1000m;
    }
}
```

- The block with a named receiver adds instance members: `order.IsHighValue`.
- The block with only a type adds static members: `Order.HighValueThreshold`.
- A generic receiver puts its type parameter on the block: `extension<T>(IReadOnlyCollection<T> items)`.
- The same static class can contain more than one block. Signatures in that class still have to be unique.

Callers see these members only when the static class's namespace is imported, the same rule as a traditional extension method.

## Methods stay compatible

A traditional extension method:

```csharp
public static bool CanCancel(this Order order) =>
    order.Status is OrderStatus.Draft or OrderStatus.Submitted;
```

can move into a block:

```csharp
extension(Order order)
{
    public bool CanCancel() =>
        order.Status is OrderStatus.Draft or OrderStatus.Submitted;
}
```

The call stays `order.CanCancel()`. Microsoft documents this method form as source compatible and binary compatible. Callers do not change.

## Properties are new

```csharp
extension(Order order)
{
    public bool IsHighValue => order.TotalAmount >= 1000m;
}
```

Callers write `order.IsHighValue`, with no parentheses. That call site needs a C# 14 compiler.

The property is computed. The compiler emits a static accessor. It does not add a field, and it cannot be an auto-property or use an `init` accessor.

A setter is allowed only as a way to write through to state the object already has:

```csharp
public string ReviewNote
{
    get => order.Note;
    set => order.Note = value.Trim();
}
```

`ReviewNote` is not new storage. `Note` is.

## What extension members cannot do

- They cannot override, hide, or replace a member declared on the type. If `Order` already has `IsHighValue`, the compiler binds to that member and ignores the extension.
- They are not virtual and they do not participate in runtime dispatch. The compile-time type of the receiver decides the binding, as with extension methods.
- They cannot be `abstract`, `virtual`, `override`, `new`, `sealed`, `partial`, or `protected`.
- They do not bypass encapsulation. Private members of `Order` stay inaccessible.
- They do not appear as declared members of `Order` when you reflect on `Order`.
- A method and a property with the same name are ambiguous if both are in scope (`CS9339`). Keep one.
- An extension property cannot appear in an expression tree (`CS9296`). `orders.Where(order => order.IsHighValue)` is valid for `IEnumerable<T>`. The same lambda cannot be an `Expression<Func<Order, bool>>`, so it is not available to `IQueryable` providers.
- A null receiver is not automatically safe. An instance member that reads the receiver will throw if the receiver is null. Check for null inside the member when null is part of the contract.
- For a struct, the receiver is a copy unless the block says `extension(ref MyStruct value)`. Without `ref`, changes inside the member are lost when the member returns.
- This repository does not claim a performance change. No benchmark is included.

## Static members

```csharp
extension(Order)
{
    public static decimal HighValueThreshold => 1000m;
}
```

`Order.HighValueThreshold` works when the extension class is in scope. A static member declared on `Order` itself wins over the extension. A block written as `extension(Order)`, with no parameter name, cannot contain instance members.

## Generic blocks

Put a type parameter on the block when the receiver uses it. Put an extra type parameter on the individual member when only that member needs it. Do not declare the receiver's type parameter in both places.

```csharp
extension<T>(IReadOnlyCollection<T> items)
{
    public bool IsEmpty => items.Count == 0;
}
```

## Official references

- [What's new in C# 14](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-14)
- [The `extension` keyword](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/extension)
- [Extension members programming guide](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods)
