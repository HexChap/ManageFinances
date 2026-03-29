using backend.Controllers;
using backend.Data;
using backend.DTOs;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace backend.Tests.Controllers;

public class IncomesControllerTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly IncomeService _service;
    private readonly IncomesController _sut;
    private const int UserId = 1;

    public IncomesControllerTests()
    {
        _db = TestDbContextFactory.Create();
        _service = new IncomeService(_db, NullLogger<IncomeService>.Instance);
        _sut = new IncomesController(_service);
        ControllerTestHelper.SetUser(_sut, UserId);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task Create_ValidRequest_Returns201WithBody()
    {
        var request = new CreateIncomeRequest(Value: 1000m);

        IActionResult result = await _sut.Create(request, CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        var body = Assert.IsType<IncomeResponse>(created.Value);
        Assert.Equal(1000m, body.Value);
        Assert.Equal(UserId, body.UserId);
    }

    [Fact]
    public async Task GetByUser_ReturnsOnlyCurrentUserIncomes()
    {
        _db.Incomes.Add(new Income { Value = 500m, UserId = 99 });
        await _db.SaveChangesAsync();
        await _sut.Create(new CreateIncomeRequest(200m), CancellationToken.None);

        IActionResult result = await _sut.GetByUser(CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsAssignableFrom<IReadOnlyList<IncomeResponse>>(ok.Value);
        Assert.Single(list);
        Assert.Equal(UserId, list[0].UserId);
    }

    [Fact]
    public async Task Update_ExistingIncome_Returns200WithUpdatedValue()
    {
        var created = (CreatedAtActionResult)(await _sut.Create(new CreateIncomeRequest(100m), CancellationToken.None));
        int id = ((IncomeResponse)created.Value!).Id;

        IActionResult result = await _sut.Update(id, new UpdateIncomeRequest(999m), CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsType<IncomeResponse>(ok.Value);
        Assert.Equal(999m, body.Value);
    }

    [Fact]
    public async Task Delete_ExistingIncome_Returns204()
    {
        var created = (CreatedAtActionResult)(await _sut.Create(new CreateIncomeRequest(50m), CancellationToken.None));
        int id = ((IncomeResponse)created.Value!).Id;

        IActionResult result = await _sut.Delete(id, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
    }
}
