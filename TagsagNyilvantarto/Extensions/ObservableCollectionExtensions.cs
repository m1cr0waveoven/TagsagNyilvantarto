using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TagsagNyilvantarto.Extensions;
internal static class ObservableCollectionExtensions
{
    public static ObservableCollection<T> ToObeservableCollection<T>(this IEnumerable<T> enumerable) => [.. enumerable];

    public static ObservableCollection<T> AddRange<T>(this ObservableCollection<T> observalbeCollection, IEnumerable<T> itemsToAdd)
    {
        itemsToAdd ??= [];
        if (observalbeCollection is null)
            return [.. itemsToAdd];

        foreach (T item in itemsToAdd)
            observalbeCollection.Add(item);

        return observalbeCollection;
    }
}
