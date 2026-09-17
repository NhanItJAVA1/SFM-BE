namespace SFM_BE.Helpers.Statistics
{
    public static class PeriodHelper
    {
        private static readonly TimeZoneInfo VietnamTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById(
            OperatingSystem.IsWindows() ? "SE Asia Standard Time" : "Asia/Ho_Chi_Minh");

        public static DateTime GetVietnamNow()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, VietnamTimeZone);
        }

        public static PeriodRange Create(
            int year,
            int month,
            bool monthToDate,
            int currentDay)
        {
            var daysInMonth = DateTime.DaysInMonth(year, month);

            var endDay = monthToDate
                ? Math.Min(currentDay, daysInMonth)
                : daysInMonth;

            var start = new DateOnly(year, month, 1);
            var end = new DateOnly(year, month, endDay);

            var startLocal = start.ToDateTime(TimeOnly.MinValue);
            var endExclusiveLocal = end.AddDays(1).ToDateTime(TimeOnly.MinValue);

            return new PeriodRange(
                month,
                year,
                start,
                end,
                TimeZoneInfo.ConvertTimeToUtc(startLocal, VietnamTimeZone),
                TimeZoneInfo.ConvertTimeToUtc(endExclusiveLocal, VietnamTimeZone));
        }
    }
}
