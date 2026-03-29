namespace backend.Models;

/// <summary>
/// Represents a single expense entry recorded by a user.
/// Expenses belong to a <see cref="Category"/> and may be labelled with zero or more <see cref="Tag"/>s.
/// </summary>
public class Expense
{
    /// <summary>Primary key.</summary>
    public int Id { get; set; }

    /// <summary>Foreign key to the associated <see cref="Category"/>.</summary>
    public int CategoryId { get; set; }

    /// <summary>Navigation property to the associated category.</summary>
    public Category Category { get; set; } = null!; // always set via FK

    /// <summary>Monetary value of the expense. Must be greater than zero.</summary>
    public decimal Value { get; set; }

    /// <summary>Foreign key to the owning <see cref="AppUser"/>.</summary>
    public int UserId { get; set; }

    /// <summary>Navigation property to the owning user.</summary>
    public AppUser User { get; set; } = null!; // always set via FK

    /// <summary>UTC timestamp of when the expense was recorded.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Tags associated with this expense (many-to-many via implicit join table).</summary>
    public ICollection<Tag> Tags { get; set; } = [];
}
