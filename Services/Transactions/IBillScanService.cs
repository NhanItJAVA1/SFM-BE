using SFM_BE.DTOs.Transactions;

namespace SFM_BE.Services.Transactions
{
    public interface IBillScanService
    {
        Task<ScanBillResponseDto> ScanAsync(IFormFile image);
    }
}
