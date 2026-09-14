using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SFM_BE.DTOs.Invoices;
using SFM_BE.Entities;
using SFM_BE.Exceptions;
using SFM_BE.Enums;
using SFM_BE.Repositories.Generic;
using SFM_BE.Repositories.UnitOfWork;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFM_BE.Services.Invoices;

public class InvoiceService : IInvoiceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IGenericRepository<Invoice> _invoiceRepo;

    public InvoiceService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _invoiceRepo = _unitOfWork.GetRepository<Invoice>();
    }

    public async Task<List<InvoiceResponseDto>> GetInvoicesAsync(long userId)
    {
        var invoices = await _invoiceRepo.Where(x => x.UserId == userId)
            .AsNoTracking()
            .ToListAsync();

        return _mapper.Map<List<InvoiceResponseDto>>(invoices);
    }

    public async Task<InvoiceResponseDto> GetInvoiceAsync(long userId, long id)
    {
        var invoice = await _invoiceRepo.Where(x => x.UserId == userId && x.Id == id)
            .AsNoTracking()
            .FirstOrDefaultAsync() ?? throw new NotFoundException("Invoice not found", "INVOICE_NOT_FOUND");

        return _mapper.Map<InvoiceResponseDto>(invoice);
    }

    public async Task CreateAsync(long userId, CreateInvoiceDto dto)
    {
        var invoice = _mapper.Map<Invoice>(dto, opt => opt.Items["UserId"] = userId);

        await _invoiceRepo.CreateAsync(invoice);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(long userId, long id, UpdateInvoiceDto dto)
    {
        var invoice = await _invoiceRepo.Where(x => x.UserId == userId && x.Id == id)
            .FirstOrDefaultAsync() ?? throw new NotFoundException("Invoice not found", "INVOICE_NOT_FOUND");

        _mapper.Map(dto, invoice);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(long userId, long id)
    {
        if(await _invoiceRepo.UpdateAsync(
            x => x.UserId == userId && x.Id == id,
            s =>
            {
                s.SetProperty(x => x.DeletedAt, System.DateTime.UtcNow);
            }) == 0)
            throw new NotFoundException("Invoice not found", "INVOICE_NOT_FOUND");
    }
}
