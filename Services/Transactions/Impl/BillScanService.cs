using SFM_BE.DTOs.Transactions;

namespace SFM_BE.Services.Transactions.Impl
{
    public class BillScanService : IBillScanService
    {
        public Task<ScanBillResponseDto> ScanAsync(IFormFile image)
        {
            throw new NotImplementedException();
        }
    }
}
