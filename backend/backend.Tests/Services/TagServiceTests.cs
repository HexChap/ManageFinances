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
        TagResponse result = await _sut.CreateAsync(new CreateTagRequest("Essential"), userId: 1);

        Assert.True(result.Id > 0);
        Assert.Equal("Essential", result.Name);
        Assert.Equal(1, result.UserId);
    }

    [Fact]
    public async Task GetByUserAsync_ReturnsAllTagsForUser()
    {
        await _sut.CreateAsync(new CreateTagRequest("Essential"), userId: 1);
        await _sut.CreateAsync(new CreateTagRequest("Recurring"), userId: 1);
        await _sut.CreateAsync(new CreateTagRequest("One-time"), userId: 1);

        IReadOnlyList<TagResponse> result = await _sut.GetByUserAsync(1);

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task GetByUserAsync_ExcludesOtherUsersTag()
    {
        await _sut.CreateAsync(new CreateTagRequest("Essential"), userId: 1);
        await _sut.CreateAsync(new CreateTagRequest("OtherUser"), userId: 2);

        IReadOnlyList<TagResponse> result = await _sut.GetByUserAsync(1);

        TagResponse single = Assert.Single(result);
        Assert.Equal("Essential", single.Name);
    }

    [Fact]
    public async Task CreateAsync_SameNameDifferentUsers_BothSucceed()
    {
        TagResponse user1Tag = await _sut.CreateAsync(new CreateTagRequest("Essential"), userId: 1);
        TagResponse user2Tag = await _sut.CreateAsync(new CreateTagRequest("Essential"), userId: 2);

        Assert.Equal("Essential", user1Tag.Name);
        Assert.Equal("Essential", user2Tag.Name);
        Assert.NotEqual(user1Tag.Id, user2Tag.Id);
    }

    [Fact]
    public async Task UpdateAsync_ExistingTag_ReturnsUpdatedData()
    {
        TagResponse created = await _sut.CreateAsync(new CreateTagRequest("Essential"), userId: 1);

        TagResponse result = await _sut.UpdateAsync(created.Id, new UpdateTagRequest("Luxury"), userId: 1);

        Assert.Equal(created.Id, result.Id);
        Assert.Equal("Luxury", result.Name);
        Assert.Equal(1, result.UserId);
    }

    [Fact]
    public async Task UpdateAsync_MissingTag_ThrowsNotFoundException()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.UpdateAsync(999, new UpdateTagRequest("X"), userId: 1));
    }

    [Fact]
    public async Task UpdateAsync_OtherUsersTag_ThrowsNotFoundException()
    {
        TagResponse created = await _sut.CreateAsync(new CreateTagRequest("Essential"), userId: 1);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.UpdateAsync(created.Id, new UpdateTagRequest("Luxury"), userId: 2));
    }

    [Fact]
    public async Task DeleteAsync_ExistingTag_Succeeds()
    {
        TagResponse created = await _sut.CreateAsync(new CreateTagRequest("Essential"), userId: 1);

        await _sut.DeleteAsync(created.Id);

        IReadOnlyList<TagResponse> remaining = await _sut.GetByUserAsync(1);
        Assert.Empty(remaining);
    }

    [Fact]
    public async Task DeleteAsync_MissingTag_ThrowsNotFoundException()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.DeleteAsync(999));
    }
}
