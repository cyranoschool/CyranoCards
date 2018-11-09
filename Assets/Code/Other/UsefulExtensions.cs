using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class UsefulExtensions {

    public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default(TValue))
    {
        if (dictionary == null) { throw new ArgumentNullException(nameof(dictionary)); }
        if (key == null) { throw new ArgumentNullException(nameof(key)); }

        TValue value;
        return dictionary.TryGetValue(key, out value) ? value : defaultValue;
    }

}

public static class EnumerableExtensions
{
    static System.Random random = new System.Random();

    public static T PickRandom<T>(this IList<T> source)
    {
        return source[random.Next(source.Count)];
    }

    public static T PickRandom<T>(this IEnumerable<T> source)
    {
        return source.PickRandom(1).Single();
    }

    public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count)
    {
        return source.Shuffle().Take(count);
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        return source.OrderBy(x => random.Next());
    }

    public static IEnumerable<T> Shuffle<T>(this List<T> source)
    {
        List<T> shuffle = new List<T>(source);
        shuffle.ShuffleInPlace<T>();
        return shuffle;
    }

    public static void ShuffleInPlace<T>(this IList<T> source)
    {
        int count = source.Count;
        for (int i = 0; i < count; i++)
        {
            int index = random.Next(count);
            T swap = source[index];
            source[index] = source[i];
            source[i] = swap;
        }
    }
}