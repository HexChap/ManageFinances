namespace backend.Models;

/// <summary>
/// Represents a single income entry recorded by a user.
/// </summary>
public class Income
{
    /// <summary>Primary key.</summary>
    public int Id { get; set; }

    /// <summary>Monetary value of the income. Must be greater than zero.</summary>
    public decimal Value { get; set; }

    /// <summary>Foreign key to the owning <see cref="AppUser"/>.</summary>
    public int UserId { get; set; }

    /// <summary>Navigation property to the owning user.</summary>
    public AppUser User { get; set; } = null!; // always set via FK

    /// <summary>UTC timestamp of when the income was recorded.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
