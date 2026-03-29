using backend.Controllers;
using backend.Data;
using backend.DTOs;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace backend.Tests.Controllers;

public class TagsControllerTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly TagService _service;
    private readonly TagsController _sut;
    private const int UserId = 1;

    public TagsControllerTests()
    {
        _db = TestDbContextFactory.Create();
        _service = new TagService(_db, NullLogger<TagService>.Instance);
        _sut = new TagsController(_service);
        ControllerTestHelper.SetUser(_sut, UserId);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task Create_ValidRequest_Returns201WithBody()
    {
        var request = new CreateTagRequest(Name: "essential");

        IActionResult result = await _sut.Create(request, CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        var body = Assert.IsType<TagResponse>(created.Value);
        Assert.Equal("essential", body.Name);
        Assert.Equal(UserId, body.UserId);
    }

    [Fact]
    public async Task GetByUser_ReturnsOnlyCurrentUserTags()
    {
        _db.Tags.Add(new Tag { Name = "other", UserId = 99 });
        await _db.SaveChangesAsync();
        await _sut.Create(new CreateTagRequest("mine"), CancellationToken.None);

        IActionResult result = await _sut.GetByUser(CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsAssignableFrom<IReadOnlyList<TagResponse>>(ok.Value);
        Assert.Single(list);
        Assert.Equal("mine", list[0].Name);
    }

    [Fact]
    public async Task Update_OwnTag_Returns200WithNewName()
    {
        var created = (CreatedAtActionResult)(await _sut.Create(new CreateTagRequest("old"), CancellationToken.None));
        int id = ((TagResponse)created.Value!).Id;

        IActionResult result = await _sut.Update(id, new UpdateTagRequest("new"), CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsType<TagResponse>(ok.Value);
        Assert.Equal("new", body.Name);
    }

    [Fact]
    public async Task Delete_ExistingTag_Returns204()
    {
        var created = (CreatedAtActionResult)(await _sut.Create(new CreateTagRequest("temp"), CancellationToken.None));
        int id = ((TagResponse)created.Value!).Id;

        IActionResult result = await _sut.Delete(id, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
    }
}
