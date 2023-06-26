namespace Bocchi.Utility;

public static class DateTimeUtility
{
    public static DateTime GetKstDateTime()
    {
        return DateTime.UtcNow.ToKstTime();
    }

    public static DateTime ToKstTime(this DateTime dateTime)
    {
        return dateTime.AddHours(9);
    }

    public static DateTimeOffset ToKstTime(this DateTimeOffset dateTime)
    {
        return dateTime.AddHours(9);
    }
}