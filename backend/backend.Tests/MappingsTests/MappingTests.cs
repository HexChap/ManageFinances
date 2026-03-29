using backend.Mappings;
using backend.Models;

namespace backend.Tests.Mappings;

public class MappingTests
{
    // ── Expense ──────────────────────────────────────────────────────────────

    [Fact]
    public void Expense_ToResponse_MapsAllFields()
    {
        var tag1 = new Tag { Id = 10, Name = "food", UserId = 1 };
        var tag2 = new Tag { Id = 20, Name = "work", UserId = 1 };
        var expense = new Expense
        {
            Id = 5,
            CategoryId = 2,
            Value = 99.99m,
            UserId = 1,
            CreatedAt = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc),
            Tags = [tag1, tag2]
        };

        var result = expense.ToResponse();

        Assert.Equal(5, result.Id);
        Assert.Equal(2, result.CategoryId);
        Assert.Equal(99.99m, result.Value);
        Assert.Equal(1, result.UserId);
        Assert.Equal(expense.CreatedAt, result.CreatedAt);
        Assert.Equal([10, 20], result.TagIds);
    }

    [Fact]
    public void Expense_ToResponse_NoTags_ReturnsEmptyTagIds()
    {
        var expense = new Expense { Id = 1, CategoryId = 1, Value = 10m, UserId = 1, Tags = [] };

        var result = expense.ToResponse();

        Assert.Empty(result.TagIds);
    }

    // ── Category ─────────────────────────────────────────────────────────────

    [Fact]
    public void Category_ToResponse_MapsAllFields()
    {
        var cat = new Category
        {
            Id = 3,
            Name = "Transport",
            UserId = 7,
            CreatedAt = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        var result = cat.ToResponse();

        Assert.Equal(3, result.Id);
        Assert.Equal("Transport", result.Name);
        Assert.Equal(7, result.UserId);
        Assert.Equal(cat.CreatedAt, result.CreatedAt);
    }

    [Fact]
    public void Category_ToResponse_NullUserId_ReturnsNullUserId()
    {
        var cat = new Category { Id = 1, Name = "Global", UserId = null };

        var result = cat.ToResponse();

        Assert.Null(result.UserId);
    }

    // ── Income ────────────────────────────────────────────────────────────────

    [Fact]
    public void Income_ToResponse_MapsAllFields()
    {
        var income = new Income
        {
            Id = 8,
            Value = 1500m,
            UserId = 2,
            CreatedAt = new DateTime(2025, 3, 10, 0, 0, 0, DateTimeKind.Utc)
        };

        var result = income.ToResponse();

        Assert.Equal(8, result.Id);
        Assert.Equal(1500m, result.Value);
        Assert.Equal(2, result.UserId);
        Assert.Equal(income.CreatedAt, result.CreatedAt);
    }

    // ── Tag ───────────────────────────────────────────────────────────────────

    [Fact]
    public void Tag_ToResponse_MapsAllFields()
    {
        var tag = new Tag { Id = 4, Name = "essential", UserId = 9 };

        var result = tag.ToResponse();

        Assert.Equal(4, result.Id);
        Assert.Equal("essential", result.Name);
        Assert.Equal(9, result.UserId);
    }
}
