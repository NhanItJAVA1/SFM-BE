using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SFM_BE.DTOs.Auth;
using SFM_BE.DTOs.Users;
using SFM_BE.Entities;
using SFM_BE.Exceptions;
using SFM_BE.Repositories.Generic;
using SFM_BE.Repositories.UnitOfWork;
using SFM_BE.Services.Auth;

namespace SFM_BE.Services.User;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IGenericRepository<Entities.User> _userRepo;
    private readonly IGenericRepository<FinancialAccount> _accountRepo;
    private readonly IGenericRepository<Transaction> _transactionRepo;
    private readonly IGenericRepository<TransactionItem> _transactionItemRepo;
    private readonly IGenericRepository<TransactionAttachment> _transactionAttachmentRepo;
    private readonly IGenericRepository<Budget> _budgetRepo;
    private readonly IGenericRepository<BudgetAlert> _budgetAlertRepo;
    private readonly IGenericRepository<Category> _categoryRepo;
    private readonly IGenericRepository<FinancialAccount> _financialAccountRepo;
    private readonly IReauthenticationService _reauthenticationService;
    private readonly S3PresignedUrlService _s3Service;

    public UserService(IUnitOfWork unitOfWork, IMapper mapper, S3PresignedUrlService s3Service, IReauthenticationService reauthenticationService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userRepo = _unitOfWork.GetRepository<Entities.User>();
        _s3Service = s3Service;
        _accountRepo = _unitOfWork.GetRepository<FinancialAccount>();
        _transactionRepo = _unitOfWork.GetRepository<Transaction>();
        _transactionItemRepo = _unitOfWork.GetRepository<TransactionItem>();
        _transactionAttachmentRepo = _unitOfWork.GetRepository<TransactionAttachment>();
        _budgetRepo = _unitOfWork.GetRepository<Budget>();
        _budgetAlertRepo = _unitOfWork.GetRepository<BudgetAlert>();
        _categoryRepo = _unitOfWork.GetRepository<Category>();
        _financialAccountRepo = _unitOfWork.GetRepository<FinancialAccount>();
        _reauthenticationService = reauthenticationService;

    }

    public async Task<List<UserResponseDto>> GetUsersAsync()
    {
        var users = await _userRepo.All()
            .Include(u => u.Role)
            .AsNoTracking()
            .ToListAsync();

        var userDtos = _mapper.Map<List<UserResponseDto>>(users);

        foreach (var userDto in userDtos)
            await ApplyAvatarReadUrlAsync(userDto);

        return userDtos;
    }

    public async Task<UserResponseDto> GetUserAsync(long id)
    {
        var user = await _userRepo.Where(u => u.Id == id)
            .Include(u => u.Role)
            .AsNoTracking()
            .FirstOrDefaultAsync() ?? throw new NotFoundException("User not found", "USER_NOT_FOUND");

        var userDto = _mapper.Map<UserResponseDto>(user);
        await ApplyAvatarReadUrlAsync(userDto);

        return userDto;
    }

    public async Task UpdateAsync(long id, UpdateUserDto dto)
    {
        var user = await _userRepo.Where(u => u.Id == id).FirstOrDefaultAsync() ?? throw new NotFoundException("User not found", "USER_NOT_FOUND");
        await EnsureEmailAvailableAsync(id, dto.Email);

        _mapper.Map(dto, user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        if (await _userRepo.UpdateAsync(x => x.Id == id && x.DeletedAt == null, s => s.SetProperty(x => x.DeletedAt, DateTime.UtcNow)) == 0)
            throw new NotFoundException("User not found", "USER_NOT_FOUND");
    }

    private async Task EnsureEmailAvailableAsync(long userId, string email)
    {
        if (await _userRepo.Where(x => x.Email == email && x.Id != userId).AsNoTracking().AnyAsync())
            throw new ConflictException("Email already exists", "EMAIL_ALREADY_EXISTS");
    }

    private async Task ApplyAvatarReadUrlAsync(UserResponseDto user)
    {
        user.AvatarUrl = await _s3Service.CreateGetUrlFromStoredUrl(user.AvatarUrl);
    }

    public async Task ResetAccountAsync(long userId, ReauthenticationDto auth)
    {
        await _reauthenticationService.VerifyAsync(userId, auth);
        await using var transaction = await _unitOfWork.BeginTransactionAsync();

        await _transactionItemRepo.DeleteAsync(x => x.Transaction.Account.UserId == userId);
        await _transactionAttachmentRepo.DeleteAsync(x =>  x.Transaction.Account.UserId == userId);
        await _transactionRepo.DeleteAsync(x => x.Account.UserId == userId);
        await _budgetAlertRepo.DeleteAsync(x => x.Budget.UserId == userId);
        await _budgetRepo.DeleteAsync(x => x.UserId == userId);
        await _accountRepo.DeleteAsync(x => x.UserId == userId);
        await _categoryRepo.DeleteAsync(x => x.UserId == userId && !x.IsDefault);
        await _financialAccountRepo.DeleteAsync(x => x.UserId == userId);

        await transaction.CommitAsync();
    }
}
