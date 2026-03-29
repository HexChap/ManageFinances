namespace backend.Models;

/// <summary>
/// Represents an expense category.
/// Categories with <see cref="UserId"/> set to <c>null</c> are global and visible to all users.
/// User-owned categories are only visible to their owner.
/// </summary>
public class Category
{
    /// <summary>Primary key.</summary>
    public int Id { get; set; }

    /// <summary>Display name of the category. Maximum 32 characters.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Foreign key to the owning <see cref="AppUser"/>.
    /// <c>null</c> indicates a global category seeded by administrators.
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>Navigation property to the owning user. <c>null</c> for global categories.</summary>
    public AppUser? User { get; set; }

    /// <summary>UTC timestamp of when the category was created.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Expenses belonging to this category.</summary>
    public ICollection<Expense> Expenses { get; set; } = [];
}
