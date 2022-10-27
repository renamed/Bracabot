namespace Bracabot2.Domain.Support
{
    public static class TimeZoneInfoExtension
    {
        public static TimeZoneInfo GetBrasiliaTimeZone()
        {
            return TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");
        }
    }
}
