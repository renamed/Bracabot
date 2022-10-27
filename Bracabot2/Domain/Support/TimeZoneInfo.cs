namespace Bracabot2.Domain.Support
{
    public static class TimeZoneInfoExtension
    {
        public static TimeZoneInfo GetBrasiliaTimeZone()
        {
            return OperatingSystem.IsWindows()
                ? TimeZoneInfo.FindSystemTimeZoneById("Brasilia")
                : TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");
        }
    }
}
