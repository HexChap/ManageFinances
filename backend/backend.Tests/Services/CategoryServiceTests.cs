using backend.Data;
using backend.DTOs;
using backend.Exceptions;
using backend.Models;
using backend.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace backend.Tests.Services;

public class CategoryServiceTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly CategoryService _sut;

    public CategoryServiceTests()
    {
        _db = TestDbContextFactory.Create();
        _sut = new CategoryService(_db, NullLogger<CategoryService>.Instance);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task CreateAsync_ReturnsCreatedCategory()
    {
        CategoryResponse result = await _sut.CreateAsync(new CreateCategoryRequest("Food"), userId: 1);

        Assert.True(result.Id > 0);
        Assert.Equal("Food", result.Name);
        Assert.Equal(1, result.UserId);
    }

    [Fact]
    public async Task GetByUserAsync_ReturnsUserSpecificAndGlobalCategories()
    {
        // Global category seeded directly (UserId = null, not creatable via API)
        _db.Categories.Add(new Category { Name = "Global" });
        await _db.SaveChangesAsync();

        await _sut.CreateAsync(new CreateCategoryRequest("Mine"), userId: 1);
        await _sut.CreateAsync(new CreateCategoryRequest("OtherUser"), userId: 2);

        IReadOnlyList<CategoryResponse> result = await _sut.GetByUserAsync(1);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, c => c.Name == "Global");
        Assert.Contains(result, c => c.Name == "Mine");
    }

    [Fact]
    public async Task GetByUserAsync_ExcludesOtherUsersCategories()
    {
        await _sut.CreateAsync(new CreateCategoryRequest("OtherUser"), userId: 99);

        IReadOnlyList<CategoryResponse> result = await _sut.GetByUserAsync(1);

        Assert.Empty(result);
    }

    [Fact]
    public async Task UpdateAsync_ExistingCategory_ReturnsUpdatedName()
    {
        CategoryResponse created = await _sut.CreateAsync(new CreateCategoryRequest("Food"), userId: 1);

        CategoryResponse result = await _sut.UpdateAsync(created.Id, new UpdateCategoryRequest("Dining"), userId: 1);

        Assert.Equal(created.Id, result.Id);
        Assert.Equal("Dining", result.Name);
        Assert.Equal(1, result.UserId);
    }

    [Fact]
    public async Task UpdateAsync_MissingCategory_ThrowsNotFoundException()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.UpdateAsync(999, new UpdateCategoryRequest("X"), userId: 1));
    }

    [Fact]
    public async Task UpdateAsync_OtherUsersCategory_ThrowsNotFoundException()
    {
        CategoryResponse created = await _sut.CreateAsync(new CreateCategoryRequest("Food"), userId: 1);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.UpdateAsync(created.Id, new UpdateCategoryRequest("Dining"), userId: 2));
    }

    [Fact]
    public async Task UpdateAsync_GlobalCategory_ThrowsNotFoundException()
    {
        _db.Categories.Add(new Category { Name = "Global" });
        await _db.SaveChangesAsync();
        Category global = _db.Categories.Single(c => c.Name == "Global");

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.UpdateAsync(global.Id, new UpdateCategoryRequest("Renamed"), userId: 1));
    }

    [Fact]
    public async Task DeleteAsync_ExistingCategory_Succeeds()
    {
        CategoryResponse created = await _sut.CreateAsync(new CreateCategoryRequest("Food"), userId: 1);

        await _sut.DeleteAsync(created.Id);

        IReadOnlyList<CategoryResponse> remaining = await _sut.GetByUserAsync(1);
        Assert.Empty(remaining);
    }

    [Fact]
    public async Task DeleteAsync_MissingCategory_ThrowsNotFoundException()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.DeleteAsync(999));
    }
}
