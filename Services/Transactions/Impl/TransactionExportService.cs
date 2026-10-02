using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using SFM_BE.DTOs.Transactions;
using SFM_BE.Entities;
using SFM_BE.Repositories.Generic;
using SFM_BE.Repositories.UnitOfWork;

namespace SFM_BE.Services.Transactions.Impl
{
    public class TransactionExportService : ITransactionExportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Transaction> _transactionRepo;
        public TransactionExportService(IGenericRepository<Transaction> transactionRepo, IUnitOfWork unitOfWork)
        {
            _transactionRepo = transactionRepo;
            _unitOfWork = unitOfWork;
        }
        public async Task<byte[]> ExportExcelAsync(long userId, DateTime? fromDate, DateTime? toDate, long? accountId)
        {
            var transactions = await GetTransactionsAsync(userId, fromDate, toDate,  accountId);
            return BuildExcel(transactions);
        }

        private async Task<List<TransactionExportDto>> GetTransactionsAsync(long userId, DateTime? fromDate, DateTime? toDate, long? accountId)
        {
            var query = _transactionRepo
                .Where(x => x.Account.UserId == userId && x.DeletedAt == null);
            
            if (fromDate.HasValue)
                query = query.Where(x => x.TransactionDate >= fromDate.Value.Date);

            if (toDate.HasValue)
            {
                var endDate = toDate.Value.Date.AddDays(1);
                query = query.Where(x => x.TransactionDate < endDate);
            }

            if (accountId.HasValue)
                query = query.Where(x => x.AccountId == accountId.Value);

            return await query
                 .AsNoTracking()
                 .OrderByDescending(x => x.TransactionDate)
                 .Select(x => new TransactionExportDto
                 {
                     TransactionDate = x.TransactionDate,
                     AccountName = x.Account.Name,
                     Type = x.Type,
                     CategoryName = x.Category != null ? x.Category.Name : "Khác",
                     Amount = x.Amount,
                     Description = x.Description,
                     Location = x.Location
                 })
                 .ToListAsync();
        }

        private static byte[] BuildExcel(IEnumerable<TransactionExportDto> transactions)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Transactions");

            AddHeader(worksheet);

            var row = 2;

            foreach (var transaction in transactions)
            {
                worksheet.Cell(row, 1).Value = transaction.TransactionDate;
                worksheet.Cell(row, 2).Value = transaction.AccountName;
                worksheet.Cell(row, 3).Value = transaction.Type.ToString();
                worksheet.Cell(row, 4).Value = transaction.CategoryName;
                worksheet.Cell(row, 5).Value = transaction.Amount;
                worksheet.Cell(row, 6).Value = transaction.Description ?? "";
                worksheet.Cell(row, 7).Value = transaction.Location ?? "";
                row++;
            }

            FormatWorksheet(worksheet);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return stream.ToArray();
        }

        private static void AddHeader(IXLWorksheet worksheet)
        {
            worksheet.Cell(1, 1).Value = "Ngày";
            worksheet.Cell(1, 2).Value = "Tài khoản";
            worksheet.Cell(1, 3).Value = "Loại";
            worksheet.Cell(1, 4).Value = "Danh mục";
            worksheet.Cell(1, 5).Value = "Số tiền";
            worksheet.Cell(1, 6).Value = "Mô tả";
            worksheet.Cell(1, 7).Value = "Địa điểm";
        }

        private static void FormatWorksheet(IXLWorksheet worksheet)
        {
            worksheet.Row(1).Style.Font.Bold = true;
            worksheet.Column(1).Style.DateFormat.Format = "dd/MM/yyyy HH:mm";
            worksheet.Column(5).Style.NumberFormat.Format = "#,##0";
            worksheet.Columns().AdjustToContents();
        }
    }
}
