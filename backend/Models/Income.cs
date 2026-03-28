namespace backend.Models;

public class Income
{
    public int Id { get; set; }
    public decimal Value { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!; // always set via FK
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
