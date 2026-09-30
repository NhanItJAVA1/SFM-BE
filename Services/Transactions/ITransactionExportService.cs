namespace SFM_BE.Services.Transactions
{
    public interface ITransactionExportService
    {
        Task<byte[]> ExportExcelAsync(long userId, DateTime? fromDate, DateTime? toDate, long? accountId);
    }
}
