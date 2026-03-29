namespace backend.Models;

/// <summary>
/// A user-defined label that can be applied to expenses for finer-grained classification.
/// Tags are scoped per user — two users may have tags with the same name independently.
/// </summary>
public class Tag
{
    /// <summary>Primary key.</summary>
    public int Id { get; set; }

    /// <summary>Display name of the tag. Maximum 32 characters.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Foreign key to the owning <see cref="AppUser"/>.</summary>
    public int UserId { get; set; }

    /// <summary>Navigation property to the owning user.</summary>
    public AppUser User { get; set; } = null!;

    /// <summary>Expenses labelled with this tag (many-to-many via implicit join table).</summary>
    public ICollection<Expense> Expenses { get; set; } = [];
}
