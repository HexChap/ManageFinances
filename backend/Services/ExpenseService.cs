using backend.Data;
using backend.DTOs;
using backend.Exceptions;
using backend.Mappings;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

/// <summary>Filtering period used when querying expenses.</summary>
public enum ExpensePeriod { All, Today, Month }

/// <summary>
/// Business logic for managing expense entries.
/// All operations are scoped to the requesting user — no user can access another user's data.
/// </summary>
public class ExpenseService
{
    private readonly AppDbContext _db;
    private readonly ILogger<ExpenseService> _logger;

    /// <param name="db">Database context.</param>
    /// <param name="logger">Logger for write operation audit trail.</param>
    public ExpenseService(AppDbContext db, ILogger<ExpenseService> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>Creates a new expense entry, optionally associating it with existing tags.</summary>
    /// <param name="request">Expense details including optional tag IDs.</param>
    /// <param name="userId">ID of the authenticated user.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created expense as a response DTO.</returns>
    public async Task<ExpenseResponse> CreateAsync(CreateExpenseRequest request, int userId, CancellationToken ct = default)
    {
        var expense = new Expense
        {
            CategoryId = request.CategoryId,
            Value = request.Value,
            UserId = userId
        };

        if (request.TagIds is { Count: > 0 })
        {
            expense.Tags = await _db.Tags
                .AsTracking()
                .Where(t => request.TagIds.Contains(t.Id))
                .ToListAsync(ct);
        }

        _db.Expenses.Add(expense);
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Expense {Id} created for user {UserId}", expense.Id, expense.UserId);
        return expense.ToResponse();
    }

    /// <summary>Returns expense entries for the given user, optionally filtered by time period.</summary>
    /// <param name="userId">ID of the authenticated user.</param>
    /// <param name="period"><see cref="ExpensePeriod.All"/> — no filter; <see cref="ExpensePeriod.Today"/> — current UTC day; <see cref="ExpensePeriod.Month"/> — current UTC month.</param>
    /// <param name="ct">Cancellation token.</param>
    public async Task<IReadOnlyList<ExpenseResponse>> GetByUserAsync(
        int userId, ExpensePeriod period, CancellationToken ct = default)
    {
        IQueryable<Expense> query = _db.Expenses
            .Include(e => e.Tags)
            .Where(e => e.UserId == userId);

        query = period switch
        {
            ExpensePeriod.Today => query.Where(e => e.CreatedAt.Date == DateTime.UtcNow.Date),
            ExpensePeriod.Month => query.Where(e =>
                e.CreatedAt.Year == DateTime.UtcNow.Year &&
                e.CreatedAt.Month == DateTime.UtcNow.Month),
            _ => query
        };

        List<Expense> expenses = await query.ToListAsync(ct);
        return expenses.Select(e => e.ToResponse()).ToList();
    }

    /// <summary>Updates an existing expense, replacing its category, value, and full set of tags.</summary>
    /// <param name="id">ID of the expense to update.</param>
    /// <param name="request">New expense details.</param>
    /// <param name="userId">ID of the authenticated user. Must match the expense owner.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated expense as a response DTO.</returns>
    /// <exception cref="NotFoundException">Thrown when the expense does not exist or belongs to a different user.</exception>
    public async Task<ExpenseResponse> UpdateAsync(int id, UpdateExpenseRequest request, int userId, CancellationToken ct = default)
    {
        Expense expense = await _db.Expenses
            .AsTracking()
            .Include(e => e.Tags)
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId, ct)
            ?? throw new NotFoundException($"Expense {id} not found");

        expense.CategoryId = request.CategoryId;
        expense.Value = request.Value;
        expense.Tags = request.TagIds is { Count: > 0 }
            ? await _db.Tags.AsTracking().Where(t => request.TagIds.Contains(t.Id)).ToListAsync(ct)
            : [];

        _db.Expenses.Update(expense);
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Expense {Id} updated", id);
        return expense.ToResponse();
    }

    /// <summary>Deletes an expense entry by ID.</summary>
    /// <param name="id">ID of the expense to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <exception cref="NotFoundException">Thrown when the expense does not exist.</exception>
    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        Expense expense = await _db.Expenses.FindAsync([id], ct)
            ?? throw new NotFoundException($"Expense {id} not found");
        _db.Expenses.Remove(expense);
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Expense {Id} deleted", id);
    }
}
