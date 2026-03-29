using backend.Controllers;
using backend.Data;
using backend.DTOs;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace backend.Tests.Controllers;

public class ExpensesControllerTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly ExpenseService _service;
    private readonly ExpensesController _sut;
    private const int UserId = 1;

    public ExpensesControllerTests()
    {
        _db = TestDbContextFactory.Create();
        _service = new ExpenseService(_db, NullLogger<ExpenseService>.Instance);
        _sut = new ExpensesController(_service);
        ControllerTestHelper.SetUser(_sut, UserId);

        _db.Categories.Add(new Category { Id = 1, Name = "Food" });
        _db.SaveChanges();
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task Create_ValidRequest_Returns201WithBody()
    {
        var request = new CreateExpenseRequest(CategoryId: 1, Value: 25m);

        IActionResult result = await _sut.Create(request, CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        var body = Assert.IsType<ExpenseResponse>(created.Value);
        Assert.Equal(25m, body.Value);
        Assert.Equal(UserId, body.UserId);
    }

    [Fact]
    public async Task GetByUser_ReturnsOnlyCurrentUserExpenses()
    {
        // seed another user's expense directly
        _db.Expenses.Add(new Expense { CategoryId = 1, Value = 99m, UserId = 99 });
        await _db.SaveChangesAsync();
        await _sut.Create(new CreateExpenseRequest(1, 10m), CancellationToken.None);

        IActionResult result = await _sut.GetByUser(ExpensePeriod.All, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsAssignableFrom<IReadOnlyList<ExpenseResponse>>(ok.Value);
        Assert.Single(list);
        Assert.All(list, e => Assert.Equal(UserId, e.UserId));
    }

    [Fact]
    public async Task Update_ExistingExpense_Returns200WithUpdatedData()
    {
        var created = (CreatedAtActionResult)(await _sut.Create(new CreateExpenseRequest(1, 20m), CancellationToken.None));
        int id = ((ExpenseResponse)created.Value!).Id;
        _db.Categories.Add(new Category { Id = 2, Name = "Transport" });
        await _db.SaveChangesAsync();

        IActionResult result = await _sut.Update(id, new UpdateExpenseRequest(2, 50m), CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsType<ExpenseResponse>(ok.Value);
        Assert.Equal(50m, body.Value);
        Assert.Equal(2, body.CategoryId);
    }

    [Fact]
    public async Task Delete_ExistingExpense_Returns204()
    {
        var created = (CreatedAtActionResult)(await _sut.Create(new CreateExpenseRequest(1, 15m), CancellationToken.None));
        int id = ((ExpenseResponse)created.Value!).Id;

        IActionResult result = await _sut.Delete(id, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
    }
}
