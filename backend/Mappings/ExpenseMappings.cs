using backend.DTOs;
using backend.Models;
using System.Linq;

namespace backend.Mappings;

/// <summary>Explicit mapping extensions for <see cref="Expense"/> entities.</summary>
public static class ExpenseMappings
{
    /// <summary>
    /// Maps an <see cref="Expense"/> entity to an <see cref="ExpenseResponse"/> DTO.
    /// Requires <see cref="Expense.Tags"/> to be loaded before calling.
    /// </summary>
    public static ExpenseResponse ToResponse(this Expense e) =>
        new(e.Id, e.CategoryId, e.Value, e.UserId, e.CreatedAt,
            e.Tags.Select(t => t.Id).ToList());
}
