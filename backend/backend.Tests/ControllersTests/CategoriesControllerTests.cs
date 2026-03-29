using backend.Controllers;
using backend.Data;
using backend.DTOs;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace backend.Tests.Controllers;

public class CategoriesControllerTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly CategoryService _service;
    private readonly CategoriesController _sut;
    private const int UserId = 1;

    public CategoriesControllerTests()
    {
        _db = TestDbContextFactory.Create();
        _service = new CategoryService(_db, NullLogger<CategoryService>.Instance);
        _sut = new CategoriesController(_service);
        ControllerTestHelper.SetUser(_sut, UserId);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task Create_ValidRequest_Returns201WithBody()
    {
        var request = new CreateCategoryRequest(Name: "Food");

        IActionResult result = await _sut.Create(request, CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        var body = Assert.IsType<CategoryResponse>(created.Value);
        Assert.Equal("Food", body.Name);
        Assert.Equal(UserId, body.UserId);
    }

    [Fact]
    public async Task GetByUser_ReturnsUserAndGlobalCategories()
    {
        // global (no owner)
        _db.Categories.Add(new Category { Name = "Global", UserId = null });
        // another user's — should NOT appear
        _db.Categories.Add(new Category { Name = "Other", UserId = 99 });
        await _db.SaveChangesAsync();

        await _sut.Create(new CreateCategoryRequest("Mine"), CancellationToken.None);

        IActionResult result = await _sut.GetByUser(CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsAssignableFrom<IReadOnlyList<CategoryResponse>>(ok.Value);
        Assert.Equal(2, list.Count);
        Assert.DoesNotContain(list, c => c.Name == "Other");
    }

    [Fact]
    public async Task Update_OwnCategory_Returns200WithNewName()
    {
        var created = (CreatedAtActionResult)(await _sut.Create(new CreateCategoryRequest("Old"), CancellationToken.None));
        int id = ((CategoryResponse)created.Value!).Id;

        IActionResult result = await _sut.Update(id, new UpdateCategoryRequest("New"), CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsType<CategoryResponse>(ok.Value);
        Assert.Equal("New", body.Name);
    }

    [Fact]
    public async Task Delete_ExistingCategory_Returns204()
    {
        var created = (CreatedAtActionResult)(await _sut.Create(new CreateCategoryRequest("ToDelete"), CancellationToken.None));
        int id = ((CategoryResponse)created.Value!).Id;

        IActionResult result = await _sut.Delete(id, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
    }
}
