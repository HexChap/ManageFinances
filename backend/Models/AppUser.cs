using Microsoft.AspNetCore.Identity;

namespace backend.Models;

public class AppUser : IdentityUser<int>
{
    public string? Timezone { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Category> Categories { get; set; } = [];
    public ICollection<Expense> Expenses { get; set; } = [];
    public ICollection<Income> Incomes { get; set; } = [];
}
