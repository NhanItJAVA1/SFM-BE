using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SFM_BE.DTOs.Transfers;
using SFM_BE.Entities;
using SFM_BE.Exceptions;
using SFM_BE.Repositories.Generic;
using SFM_BE.Repositories.UnitOfWork;

namespace SFM_BE.Services.Transfers;

public class TransferService : ITransferService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IGenericRepository<Transfer> _transferRepo;

    public TransferService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _transferRepo = _unitOfWork.GetRepository<Transfer>();
    }

    public async Task<List<TransferResponseDto>> GetTransfersAsync(long userId)
    {
        var transfers = await _transferRepo.Where(x => x.UserId == userId)
            .AsNoTracking()
            .ToListAsync();

        return _mapper.Map<List<TransferResponseDto>>(transfers);
    }

    public async Task<TransferResponseDto> GetTransferAsync(long userId, long id)
    {
        var transfer = await _transferRepo.Where(x => x.UserId == userId && x.Id == id)
            .AsNoTracking()
            .FirstOrDefaultAsync() ?? throw new NotFoundException("Transfer not found", "TRANSFER_NOT_FOUND");

        return _mapper.Map<TransferResponseDto>(transfer);
    }

    public async Task CreateAsync(long userId, CreateTransferDto dto)
    {
        var transfer = _mapper.Map<Transfer>(dto, opt => opt.Items["UserId"] = userId);

        await _transferRepo.CreateAsync(transfer);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(long userId, long id)
    {
        if (await _transferRepo.DeleteAsync(x => x.UserId == userId && x.Id == id) == 0)
            throw new NotFoundException("Transfer not found", "TRANSFER_NOT_FOUND");
    }
}
