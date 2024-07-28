using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TagsagNyilvantarto.Extensions;
internal static class ObservableCollectionExtensions
{
    public static ObservableCollection<T> ToObeservableCollection<T>(this IEnumerable<T> enumerable) => new ObservableCollection<T>(enumerable);

    public static ObservableCollection<T> AddRange<T>(this ObservableCollection<T> observalbeCollection, IEnumerable<T> itemToAdd)
    {
        itemToAdd ??= Enumerable.Empty<T>();
        if (observalbeCollection is null || observalbeCollection.Count == 0)
            return new ObservableCollection<T>(itemToAdd);

        foreach (T item in itemToAdd)
        {
            observalbeCollection.Add(item);
        }
        return observalbeCollection;
    }
}
