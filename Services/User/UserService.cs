using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SFM_BE.DTOs.Users;
using SFM_BE.Exceptions;
using SFM_BE.Repositories.Generic;
using SFM_BE.Repositories.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            .FirstOrDefaultAsync();

        if (user == null)
            throw new NotFoundException("User not found", "USER_NOT_FOUND");

        return _mapper.Map<UserResponseDto>(user);
    }

    public async Task UpdateAsync(long id, UpdateUserDto dto)
    {
        var user = await _userRepo.Where(u => u.Id == id)
            .FirstOrDefaultAsync();

        if (user == null)
            throw new NotFoundException("User not found", "USER_NOT_FOUND");

        await EnsureEmailAvailableAsync(id, dto.Email);

        _mapper.Map(dto, user);
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepo.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var user = await _userRepo.FindAsync(u => u.Id == id);

        if (user == null)
            throw new NotFoundException("User not found", "USER_NOT_FOUND");

        await _userRepo.DeleteAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task EnsureEmailAvailableAsync(long userId, string email)
    {
        var exists = await _userRepo.Where(u => u.Email == email && u.Id != userId)
            .AsNoTracking()
            .AnyAsync();

        if (exists)
            throw new ConflictException("Email already exists", "EMAIL_ALREADY_EXISTS");
    }
}
