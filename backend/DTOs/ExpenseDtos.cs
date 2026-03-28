using System.ComponentModel.DataAnnotations;

namespace backend.DTOs;

public record CreateExpenseRequest(
    [Range(1, int.MaxValue)] int CategoryId,
    [Range(0.01, double.MaxValue, ErrorMessage = "Value must be positive")] decimal Value,
    [Range(1, int.MaxValue)] int UserId,
    IReadOnlyList<int>? TagIds = null
);

public record ExpenseResponse(int Id, int CategoryId, decimal Value, int UserId, DateTime CreatedAt, IReadOnlyList<int> TagIds);
