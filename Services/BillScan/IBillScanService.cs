using SFM_BE.DTOs.Transactions;

namespace SFM_BE.Services.BillScan
{
    public interface IBillScanService
    {
        Task<ScanBillResponseDto> ScanAsync(long userId, IFormFile image);
    }
}
