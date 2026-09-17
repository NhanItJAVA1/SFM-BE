namespace SFM_BE.DTOs.Transactions.Statistics
{
    public class CategorySpendingResponseDto
    {
        public PeriodDto CurrentPeriod { get; set; } = null!;
        public PeriodDto ComparePeriod { get; set; } = null!;

        public decimal TotalAmount { get; set; }
        public decimal CompareTotalAmount { get; set; }
        public decimal? TotalChangePercentage { get; set; }

        public List<CategorySpendingDto> Categories { get; set; } = [];
    }
}
