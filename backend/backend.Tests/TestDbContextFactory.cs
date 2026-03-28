using backend.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Tests;

public static class TestDbContextFactory
{
    public static AppDbContext Create()
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}
