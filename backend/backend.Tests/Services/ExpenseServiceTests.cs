using backend.Data;
using backend.DTOs;
using backend.Exceptions;
using backend.Models;
using backend.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace backend.Tests.Services;

public class ExpenseServiceTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly ExpenseService _sut;

    public ExpenseServiceTests()
    {
        _db = TestDbContextFactory.Create();
        _sut = new ExpenseService(_db, NullLogger<ExpenseService>.Instance);

        // Seed a category required by FK
        _db.Categories.Add(new Category { Id = 1, Name = "Food" });
        _db.SaveChanges();
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task CreateAsync_ReturnsCreatedExpense()
    {
        ExpenseResponse result = await _sut.CreateAsync(new CreateExpenseRequest(1, 25.50m), userId: 1);

        Assert.True(result.Id > 0);
        Assert.Equal(25.50m, result.Value);
        Assert.Equal(1, result.CategoryId);
        Assert.Equal(1, result.UserId);
    }

    [Fact]
    public async Task GetByUserAsync_All_ReturnsAllUserExpenses()
    {
        await _sut.CreateAsync(new CreateExpenseRequest(1, 10m), userId: 1);
        await _sut.CreateAsync(new CreateExpenseRequest(1, 20m), userId: 1);
        await _sut.CreateAsync(new CreateExpenseRequest(1, 30m), userId: 2); // different user

        IReadOnlyList<ExpenseResponse> result = await _sut.GetByUserAsync(1, ExpensePeriod.All);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetByUserAsync_Today_ReturnsOnlyTodayExpenses()
    {
        await _sut.CreateAsync(new CreateExpenseRequest(1, 10m), userId: 1);

        // Seed an old expense directly
        _db.Expenses.Add(new Expense
        {
            CategoryId = 1,
            Value = 99m,
            UserId = 1,
            CreatedAt = DateTime.UtcNow.AddMonths(-1)
        });
        await _db.SaveChangesAsync();

        IReadOnlyList<ExpenseResponse> result = await _sut.GetByUserAsync(1, ExpensePeriod.Today);

        Assert.Single(result);
        Assert.Equal(10m, result[0].Value);
    }

    [Fact]
    public async Task GetByUserAsync_Month_ReturnsCurrentMonthExpenses()
    {
        await _sut.CreateAsync(new CreateExpenseRequest(1, 10m), userId: 1);

        _db.Expenses.Add(new Expense
        {
            CategoryId = 1,
            Value = 99m,
            UserId = 1,
            CreatedAt = DateTime.UtcNow.AddYears(-1)
        });
        await _db.SaveChangesAsync();

        IReadOnlyList<ExpenseResponse> result = await _sut.GetByUserAsync(1, ExpensePeriod.Month);

        Assert.Single(result);
    }

    [Fact]
    public async Task UpdateAsync_ExistingExpense_ReturnsUpdatedData()
    {
        _db.Categories.Add(new Category { Id = 2, Name = "Transport" });
        await _db.SaveChangesAsync();
        ExpenseResponse created = await _sut.CreateAsync(new CreateExpenseRequest(1, 25m), userId: 1);

        ExpenseResponse result = await _sut.UpdateAsync(created.Id, new UpdateExpenseRequest(2, 50m), userId: 1);

        Assert.Equal(created.Id, result.Id);
        Assert.Equal(2, result.CategoryId);
        Assert.Equal(50m, result.Value);
        Assert.Empty(result.TagIds);
    }

    [Fact]
    public async Task UpdateAsync_MissingExpense_ThrowsNotFoundException()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.UpdateAsync(999, new UpdateExpenseRequest(1, 10m), userId: 1));
    }

    [Fact]
    public async Task UpdateAsync_OtherUsersExpense_ThrowsNotFoundException()
    {
        ExpenseResponse created = await _sut.CreateAsync(new CreateExpenseRequest(1, 25m), userId: 1);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.UpdateAsync(created.Id, new UpdateExpenseRequest(1, 50m), userId: 2));
    }

    [Fact]
    public async Task DeleteAsync_ExistingExpense_Succeeds()
    {
        ExpenseResponse created = await _sut.CreateAsync(new CreateExpenseRequest(1, 10m), userId: 1);

        await _sut.DeleteAsync(created.Id);

        IReadOnlyList<ExpenseResponse> remaining = await _sut.GetByUserAsync(1, ExpensePeriod.All);
        Assert.Empty(remaining);
    }

    [Fact]
    public async Task DeleteAsync_MissingExpense_ThrowsNotFoundException()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.DeleteAsync(999));
    }
}
