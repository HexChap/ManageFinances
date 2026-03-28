using backend.Data;
using backend.DTOs;
using backend.Exceptions;
using backend.Mappings;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class UserService
{
    private readonly AppDbContext _db;
    private readonly ILogger<UserService> _logger;

    public UserService(AppDbContext db, ILogger<UserService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken ct = default)
    {
        var user = new User { Timezone = request.Timezone };
        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("User {Id} created", user.Id);
        return user.ToResponse();
    }

    public async Task<UserResponse> GetByIdAsync(int id, CancellationToken ct = default)
    {
        User user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct)
            ?? throw new NotFoundException($"User {id} not found");
        return user.ToResponse();
    }

    public async Task<UserResponse> UpdateAsync(int id, UpdateUserRequest request, CancellationToken ct = default)
    {
        User user = await _db.Users.AsTracking().FirstOrDefaultAsync(u => u.Id == id, ct)
            ?? throw new NotFoundException($"User {id} not found");

        if (request.Timezone is not null)
            user.Timezone = request.Timezone;

        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("User {Id} updated", id);
        return user.ToResponse();
    }
}
