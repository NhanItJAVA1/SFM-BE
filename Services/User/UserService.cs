using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SFM_BE.DTOs.Users;
using SFM_BE.Exceptions;
using SFM_BE.Repositories.Generic;
using SFM_BE.Repositories.UnitOfWork;

namespace SFM_BE.Services.User;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IGenericRepository<Entities.User> _userRepo;

    public UserService(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userRepo = _unitOfWork.GetRepository<Entities.User>();
    }

    public async Task<List<UserResponseDto>> GetUsersAsync()
    {
        var users = await _userRepo.All()
            .Include(u => u.Role)
            .AsNoTracking()
            .ToListAsync();

        return _mapper.Map<List<UserResponseDto>>(users);
    }

    public async Task<UserResponseDto> GetUserAsync(long id)
    {
        var user = await _userRepo.Where(u => u.Id == id)
            .Include(u => u.Role)
            .AsNoTracking()
            .FirstOrDefaultAsync() ?? throw new NotFoundException("User not found", "USER_NOT_FOUND");

        return _mapper.Map<UserResponseDto>(user);
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
        if (await _userRepo.UpdateAsync(
        x => x.Id == id && x.DeletedAt == null,
        s => s.SetProperty(x => x.DeletedAt, DateTime.UtcNow)) == 0)
            throw new NotFoundException("User not found", "USER_NOT_FOUND");
    }

    private async Task EnsureEmailAvailableAsync(long userId, string email)
    {
        if (await _userRepo.Where(x => x.Email == email && x.Id != userId).AsNoTracking().AnyAsync())
            throw new ConflictException("Email already exists", "EMAIL_ALREADY_EXISTS");
    }
}
