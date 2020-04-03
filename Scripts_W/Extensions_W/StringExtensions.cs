namespace Wichtel.Extensions{
public static class StringExtensions
{
    public static string Substring(this string stringToGetSubstringFrom, string beginAfter, string endAt)
    {
        int pFrom = stringToGetSubstringFrom.IndexOf(beginAfter) + beginAfter.Length;
        int pTo = stringToGetSubstringFrom.LastIndexOf(endAt);

        return stringToGetSubstringFrom.Substring(pFrom, pTo - pFrom);
    }
}
}