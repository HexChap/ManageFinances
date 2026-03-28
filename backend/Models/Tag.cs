namespace backend.Models;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int UserId { get; set; }
    public AppUser User { get; set; } = null!;
    public ICollection<Expense> Expenses { get; set; } = [];
}
