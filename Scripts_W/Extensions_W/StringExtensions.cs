using System;

namespace Wichtel.Extensions{
public static class StringExtensions
{
    public static string Substring(this string stringToGetSubstringFrom, string beginAfter, string endAt)
    {
        int pFrom = stringToGetSubstringFrom.IndexOf(beginAfter) + beginAfter.Length;
        int pTo = stringToGetSubstringFrom.LastIndexOf(endAt);

        return stringToGetSubstringFrom.Substring(pFrom, pTo - pFrom);
    }

    public static bool IsNullOrEmptyOrWhiteSpace(this string _string)
    {
        if(String.IsNullOrEmpty(_string)) return true;
        if(String.IsNullOrWhiteSpace(_string)) return true;
        return false;
    }
}
}