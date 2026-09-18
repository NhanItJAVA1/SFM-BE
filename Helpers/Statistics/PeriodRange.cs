namespace SFM_BE.Helpers.Statistics
{
    public record PeriodRange(int Month, int Year, DateOnly Start, DateOnly End, DateTime StartUtc, DateTime EndUtcExclusive)
    {
    }
}
