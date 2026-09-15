using SFM_BE.DTOs.Invoices;
using SFM_BE.Enums;

namespace SFM_BE.Services.Invoices;

public interface IInvoiceService
{
    Task<List<InvoiceResponseDto>> GetInvoicesAsync(long userId, DeleteType filter = DeleteType.NotDeleted);

    Task<InvoiceResponseDto> GetInvoiceAsync(long userId, long id);

    Task CreateAsync(long userId, CreateInvoiceDto dto);

    Task UpdateAsync(long userId, long id, UpdateInvoiceDto dto);

    Task DeleteSoftAsync(long userId, long id);
}
