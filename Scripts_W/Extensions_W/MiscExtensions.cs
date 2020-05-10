using System;
using System.Collections.Generic;
using System.Linq;

namespace Wichtel.Extensions {
public static class MiscExtensions
{
    // Ex: collection.TakeLast(5);
    public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int n)
    {
        return source.Skip(Math.Max(0, source.Count() - n));
    }
    
    //Populate array with one value
    public static void Populate<T>(this T[] arr, T value )
    {
        for ( int i = 0; i < arr.Length;i++ )
        {
            arr[i] = value;
        }
    }
}
}