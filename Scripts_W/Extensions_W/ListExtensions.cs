using System.Collections.Generic;

namespace Wichtel.Extensions{
public static class ListExtensions
{
    private static void RemoveEmptyEntries<T>(this List<T> _list) where T : class
    {
        _list.RemoveAll(item => item == null);
    }
}
}