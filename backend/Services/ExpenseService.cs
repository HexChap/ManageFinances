using backend.Data;
using backend.DTOs;
using backend.Exceptions;
using backend.Mappings;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public enum ExpensePeriod { All, Today, Month }

public class ExpenseService
{
    private readonly AppDbContext _db;
    private readonly ILogger<ExpenseService> _logger;

    public ExpenseService(AppDbContext db, ILogger<ExpenseService> logger)
    {
        _db = db;
        _logger = logger;
    }

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

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        Expense expense = await _db.Expenses.FindAsync([id], ct)
            ?? throw new NotFoundException($"Expense {id} not found");
        _db.Expenses.Remove(expense);
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Expense {Id} deleted", id);
    }
}
