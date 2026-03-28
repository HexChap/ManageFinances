namespace backend.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? UserId { get; set; }
    public AppUser? User { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Expense> Expenses { get; set; } = [];
}
