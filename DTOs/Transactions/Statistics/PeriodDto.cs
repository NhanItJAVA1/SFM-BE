namespace SFM_BE.DTOs.Transactions.Statistics
{
    public class PeriodDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public DateOnly Start { get; set; }
        public DateOnly End { get; set; }
    }
}
