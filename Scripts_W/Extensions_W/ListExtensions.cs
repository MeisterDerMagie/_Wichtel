using System.Collections.Generic;

namespace Wichtel.Extensions{
public static class ListExtensions
{
    private static void RemoveEmptyEntries<T>(this List<T> _list) where T : class
    {
        for (int i = _list.Count-1; i >= 0; i--)
        {
            if(_list[i] == null) _list.RemoveAt(i);
        }
    }
}
}