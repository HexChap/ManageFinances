using Microsoft.AspNetCore.Identity;

namespace backend.Models;

/// <summary>
/// Application user extending ASP.NET Core Identity with finance-specific fields.
/// </summary>
public class AppUser : IdentityUser<int>
{
    /// <summary>IANA timezone identifier for the user, e.g. "Europe/Sofia". Null means UTC.</summary>
    public string? Timezone { get; set; }

    /// <summary>UTC timestamp of account creation.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Categories created by this user.</summary>
    public ICollection<Category> Categories { get; set; } = [];

    /// <summary>Expenses recorded by this user.</summary>
    public ICollection<Expense> Expenses { get; set; } = [];

    /// <summary>Incomes recorded by this user.</summary>
    public ICollection<Income> Incomes { get; set; } = [];
}
