using System.Text;

namespace Bracabot2.Domain.Support
{
    public static class TimespanExtensions
    {
        private const int SECONDS_WEEK = 604800;

        public static string GetReadable(this TimeSpan timeSpan)
        {
            long totalSeconds = (long)timeSpan.TotalSeconds;

            int weeks = (int)totalSeconds / SECONDS_WEEK;
            int days = (int)timeSpan.TotalDays % 7;
            int hours = timeSpan.Hours;
            int minutes = timeSpan.Minutes;
            int seconds = timeSpan.Seconds;

            var sb = new StringBuilder();

            AddIf(weeks, "semana", sb);
            AddIf(days, "dia", sb);
            AddIf(hours, "hora", sb);
            AddIf(minutes, "minuto", sb);
            AddIf(seconds, "segundo", sb, sb.Length == 0);

            sb.Length -= 2;

            return sb.ToString().Trim();
        }

        private static void AddIf(int value, string identifier, StringBuilder sb, bool addZero = false)
        {
            if ((!addZero && value > 0) || (addZero))
            {
                sb.Append(value);
                sb.Append(' ');
                sb.Append(identifier);
                if (value > 1)
                    sb.Append('s');

                sb.Append(", ");
            }
        }
    }
}
