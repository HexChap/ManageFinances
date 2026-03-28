using backend.Data;
using backend.DTOs;
using backend.Exceptions;
using backend.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace backend.Tests.Services;

public class TagServiceTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly TagService _sut;

    public TagServiceTests()
    {
        _db = TestDbContextFactory.Create();
        _sut = new TagService(_db, NullLogger<TagService>.Instance);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task CreateAsync_ReturnsCorrectData()
    {
        TagResponse result = await _sut.CreateAsync(new CreateTagRequest("Essential", null));

        Assert.True(result.Id > 0);
        Assert.Equal("Essential", result.Name);
        Assert.Equal(1, result.UserId); 
    } 

    [Fact]
    public async Task GetByUserAsync_ReturnsAllTagsForUser()
    {
        await _sut.CreateAsync(new CreateTagRequest("Essential", 1));
        await _sut.CreateAsync(new CreateTagRequest("Recurring", 1));
        await _sut.CreateAsync(new CreateTagRequest("One-time", 1));

        IReadOnlyList<TagResponse> result = await _sut.GetByUserAsync(1);

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task CreateAsync_SameNameDifferentUsers_BothSucceed()
    {
        TagResponse user1Tag = await _sut.CreateAsync(new CreateTagRequest("Essential", 1));
        TagResponse user2Tag = await _sut.CreateAsync(new CreateTagRequest("Essential", 2));

        Assert.Equal("Essential", user1Tag.Name);
        Assert.Equal("Essential", user2Tag.Name);
        Assert.NotEqual(user1Tag.Id, user2Tag.Id);
    }

    [Fact]
    public async Task DeleteAsync_MissingTag_ThrowsNotFoundException()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.DeleteAsync(999));
    }
}