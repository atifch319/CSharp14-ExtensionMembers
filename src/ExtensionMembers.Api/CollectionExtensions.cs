namespace ExtensionMembers.Api;

public static class CollectionExtensions
{
    extension<T>(IReadOnlyCollection<T> items)
    {
        public bool IsEmpty => items.Count == 0;
    }
}
