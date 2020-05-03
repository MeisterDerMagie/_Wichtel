using System;

namespace Wichtel.Extensions
{
public static class DateTimeExtensions
{
    public static TimeSpan GetDuration(DateTime _olderDate, DateTime _youngerDate)
    {
        return _youngerDate.Subtract(_olderDate);
    }
}
}