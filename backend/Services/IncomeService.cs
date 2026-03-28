using backend.Data;
using backend.DTOs;
using backend.Exceptions;
using backend.Mappings;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class IncomeService
{
    private readonly AppDbContext _db;
    private readonly ILogger<IncomeService> _logger;

    public IncomeService(AppDbContext db, ILogger<IncomeService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IncomeResponse> CreateAsync(CreateIncomeRequest request, int userId, CancellationToken ct = default)
    {
        var income = new Income { Value = request.Value, UserId = userId };
        _db.Incomes.Add(income);
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Income {Id} created for user {UserId}", income.Id, income.UserId);
        return income.ToResponse();
    }

    public async Task<IReadOnlyList<IncomeResponse>> GetByUserAsync(int userId, CancellationToken ct = default)
    {
        List<Income> incomes = await _db.Incomes
            .Where(i => i.UserId == userId)
            .ToListAsync(ct);
        return incomes.Select(i => i.ToResponse()).ToList();
    }

    public async Task<IncomeResponse> UpdateAsync(int id, UpdateIncomeRequest request, int userId, CancellationToken ct = default)
    {
        Income income = await _db.Incomes.AsTracking().FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId, ct)
            ?? throw new NotFoundException($"Income {id} not found");
        income.Value = request.Value;
        _db.Incomes.Update(income);
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Income {Id} updated", id);
        return income.ToResponse();
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        Income income = await _db.Incomes.FindAsync([id], ct)
            ?? throw new NotFoundException($"Income {id} not found");
        _db.Incomes.Remove(income);
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Income {Id} deleted", id);
    }
}
