namespace backend.Models;

public class Expense
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!; // always set via FK
    public decimal Value { get; set; }
    public int UserId { get; set; }
    public AppUser User { get; set; } = null!; // always set via FK
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Tag> Tags { get; set; } = [];
}
