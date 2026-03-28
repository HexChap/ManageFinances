using backend.DTOs;
using backend.Models;

namespace backend.Mappings;

public static class ExpenseMappings
{
    public static ExpenseResponse ToResponse(this Expense e) =>
        new(e.Id, e.CategoryId, e.Value, e.UserId, e.CreatedAt);
}
