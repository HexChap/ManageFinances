using backend.Data;
using backend.DTOs;
using backend.Exceptions;
using backend.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace backend.Tests.Services;

public class UserServiceTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _db = TestDbContextFactory.Create();
        _sut = new UserService(_db, NullLogger<UserService>.Instance);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task CreateAsync_ReturnsCreatedUser()
    {
        UserResponse result = await _sut.CreateAsync(new CreateUserRequest("Europe/Sofia"));

        Assert.True(result.Id > 0);
        Assert.Equal("Europe/Sofia", result.Timezone);
    }

    [Fact]
    public async Task CreateAsync_WithNullTimezone_ReturnsUserWithNullTimezone()
    {
        UserResponse result = await _sut.CreateAsync(new CreateUserRequest(null));

        Assert.Null(result.Timezone);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingUser_ReturnsUser()
    {
        UserResponse created = await _sut.CreateAsync(new CreateUserRequest("UTC"));

        UserResponse result = await _sut.GetByIdAsync(created.Id);

        Assert.Equal(created.Id, result.Id);
        Assert.Equal("UTC", result.Timezone);
    }

    [Fact]
    public async Task GetByIdAsync_MissingUser_ThrowsNotFoundException()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetByIdAsync(999));
    }

    [Fact]
    public async Task UpdateAsync_ExistingUser_UpdatesTimezone()
    {
        UserResponse created = await _sut.CreateAsync(new CreateUserRequest("UTC"));

        UserResponse result = await _sut.UpdateAsync(created.Id, new UpdateUserRequest("Europe/Sofia"));

        Assert.Equal("Europe/Sofia", result.Timezone);
    }

    [Fact]
    public async Task UpdateAsync_NullTimezone_DoesNotOverwrite()
    {
        UserResponse created = await _sut.CreateAsync(new CreateUserRequest("UTC"));

        UserResponse result = await _sut.UpdateAsync(created.Id, new UpdateUserRequest(null));

        Assert.Equal("UTC", result.Timezone);
    }

    [Fact]
    public async Task UpdateAsync_MissingUser_ThrowsNotFoundException()
    {
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _sut.UpdateAsync(999, new UpdateUserRequest("UTC")));
    }
}
