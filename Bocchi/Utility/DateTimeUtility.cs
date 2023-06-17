namespace Bocchi.Utility;

public static class DateTimeUtility
{
    public static DateTime GetKstDateTime()
    {
        return DateTime.UtcNow.AddHours(9);
    }
}