using backend.Data;
using backend.DTOs;
using backend.Exceptions;
using backend.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace backend.Tests.Services;

public class IncomeServiceTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly IncomeService _sut;

    public IncomeServiceTests()
    {
        _db = TestDbContextFactory.Create();
        _sut = new IncomeService(_db, NullLogger<IncomeService>.Instance);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task CreateAsync_ReturnsCreatedIncome()
    {
        IncomeResponse result = await _sut.CreateAsync(new CreateIncomeRequest(1500m, 1));

        Assert.True(result.Id > 0);
        Assert.Equal(1500m, result.Value);
        Assert.Equal(1, result.UserId);
    }

    [Fact]
    public async Task GetByUserAsync_ReturnsOnlyUserIncomes()
    {
        await _sut.CreateAsync(new CreateIncomeRequest(1000m, 1));
        await _sut.CreateAsync(new CreateIncomeRequest(2000m, 1));
        await _sut.CreateAsync(new CreateIncomeRequest(3000m, 2)); // different user

        IReadOnlyList<IncomeResponse> result = await _sut.GetByUserAsync(1);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetByUserAsync_NoIncomes_ReturnsEmptyList()
    {
        IReadOnlyList<IncomeResponse> result = await _sut.GetByUserAsync(99);

        Assert.Empty(result);
    }

    [Fact]
    public async Task DeleteAsync_ExistingIncome_Succeeds()
    {
        IncomeResponse created = await _sut.CreateAsync(new CreateIncomeRequest(500m, 1));

        await _sut.DeleteAsync(created.Id);

        IReadOnlyList<IncomeResponse> remaining = await _sut.GetByUserAsync(1);
        Assert.Empty(remaining);
    }

    [Fact]
    public async Task DeleteAsync_MissingIncome_ThrowsNotFoundException()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.DeleteAsync(999));
    }
}
