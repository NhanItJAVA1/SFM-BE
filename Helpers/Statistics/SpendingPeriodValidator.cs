using SFM_BE.Exceptions;

namespace SFM_BE.Helpers.Statistics
{
    public static class SpendingPeriodValidator
    {
        public static void Validate(int? month, int? year, int? compareMonth, int? compareYear)
        {
            if (month.HasValue != year.HasValue)
                throw new BadRequestException(
                    "Month and year must be provided together.",
                    "INVALID_PERIOD");

            if (compareMonth.HasValue != compareYear.HasValue)
                throw new BadRequestException(
                    "Compare month and year must be provided together.",
                    "INVALID_COMPARE_PERIOD");

            if (month is < 1 or > 12)
                throw new BadRequestException(
                    "Month must be between 1 and 12.",
                    "INVALID_MONTH");

            if (compareMonth is < 1 or > 12)
                throw new BadRequestException(
                    "Compare month must be between 1 and 12.",
                    "INVALID_COMPARE_MONTH");
        }

        public static void ValidatePeriod(DateOnly currentPeriod, DateOnly comparePeriod, DateOnly actualPeriod)
        {
            if (currentPeriod > actualPeriod)
                throw new BadRequestException(
                    "Current period cannot be in the future.",
                    "INVALID_PERIOD");

            if (comparePeriod > actualPeriod)
                throw new BadRequestException(
                    "Compare period cannot be in the future.",
                    "INVALID_COMPARE_PERIOD");

            if (currentPeriod == comparePeriod)
                throw new BadRequestException(
                    "Compare period cannot be the same as current period.",
                    "SAME_COMPARE_PERIOD");
        }
    }
}
