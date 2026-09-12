using SFM_BE.DTOs.Transfers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFM_BE.Services.Transfers;

public interface ITransferService
{
    Task<List<TransferResponseDto>> GetTransfersAsync(long userId);

    Task<TransferResponseDto> GetTransferAsync(long userId, long id);

    Task<TransferResponseDto> CreateAsync(long userId, CreateTransferDto dto);

    Task DeleteAsync(long userId, long id);
}
