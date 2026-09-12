using SFM_BE.DTOs.Invoices;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFM_BE.Services.Invoices;

public interface IInvoiceService
{
    Task<List<InvoiceResponseDto>> GetInvoicesAsync(long userId);

    Task<InvoiceResponseDto> GetInvoiceAsync(long userId, long id);

    Task<InvoiceResponseDto> CreateAsync(long userId, CreateInvoiceDto dto);

    Task UpdateAsync(long userId, long id, UpdateInvoiceDto dto);

    Task DeleteAsync(long userId, long id);
}
