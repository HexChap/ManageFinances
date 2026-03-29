using backend.Data;
using backend.DTOs;
using backend.Exceptions;
using backend.Mappings;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

/// <summary>
/// Business logic for managing income entries.
/// All operations are scoped to the requesting user — no user can access another user's data.
/// </summary>
public class IncomeService
{
    private readonly AppDbContext _db;
    private readonly ILogger<IncomeService> _logger;

    /// <param name="db">Database context.</param>
    /// <param name="logger">Logger for write operation audit trail.</param>
    public IncomeService(AppDbContext db, ILogger<IncomeService> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>Creates a new income entry for the given user.</summary>
    /// <param name="request">Income details.</param>
    /// <param name="userId">ID of the authenticated user.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created income as a response DTO.</returns>
    public async Task<IncomeResponse> CreateAsync(CreateIncomeRequest request, int userId, CancellationToken ct = default)
    {
        var income = new Income { Value = request.Value, UserId = userId };
        _db.Incomes.Add(income);
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Income {Id} created for user {UserId}", income.Id, income.UserId);
        return income.ToResponse();
    }

    /// <summary>Returns all income entries belonging to the given user.</summary>
    /// <param name="userId">ID of the authenticated user.</param>
    /// <param name="ct">Cancellation token.</param>
    public async Task<IReadOnlyList<IncomeResponse>> GetByUserAsync(int userId, CancellationToken ct = default)
    {
        List<Income> incomes = await _db.Incomes
            .Where(i => i.UserId == userId)
            .ToListAsync(ct);
        return incomes.Select(i => i.ToResponse()).ToList();
    }

    /// <summary>Updates the value of an existing income entry.</summary>
    /// <param name="id">ID of the income to update.</param>
    /// <param name="request">New income details.</param>
    /// <param name="userId">ID of the authenticated user. Must match the income owner.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated income as a response DTO.</returns>
    /// <exception cref="NotFoundException">Thrown when the income does not exist or belongs to a different user.</exception>
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

    /// <summary>Deletes an income entry by ID.</summary>
    /// <param name="id">ID of the income to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <exception cref="NotFoundException">Thrown when the income does not exist.</exception>
    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        Income income = await _db.Incomes.FindAsync([id], ct)
            ?? throw new NotFoundException($"Income {id} not found");
        _db.Incomes.Remove(income);
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Income {Id} deleted", id);
    }
}
