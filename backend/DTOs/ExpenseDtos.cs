using System.ComponentModel.DataAnnotations;

namespace backend.DTOs;

/// <summary>Request body for creating a new expense entry.</summary>
public record CreateExpenseRequest(
    /// <summary>ID of the category this expense belongs to.</summary>
    [Range(1, int.MaxValue)] int CategoryId,
    /// <summary>Monetary value. Must be greater than zero.</summary>
    [Range(0.01, double.MaxValue, ErrorMessage = "Value must be positive")] decimal Value,
    /// <summary>Optional list of tag IDs to associate with the expense.</summary>
    IReadOnlyList<int>? TagIds = null
);

/// <summary>Request body for updating an existing expense entry.</summary>
public record UpdateExpenseRequest(
    /// <summary>ID of the new category for this expense.</summary>
    [Range(1, int.MaxValue)] int CategoryId,
    /// <summary>New monetary value. Must be greater than zero.</summary>
    [Range(0.01, double.MaxValue, ErrorMessage = "Value must be positive")] decimal Value,
    /// <summary>Replaces the full set of tag associations. Pass null or empty to remove all tags.</summary>
    IReadOnlyList<int>? TagIds = null
);

/// <summary>Read model returned for expense entries.</summary>
public record ExpenseResponse(int Id, int CategoryId, decimal Value, int UserId, DateTime CreatedAt, IReadOnlyList<int> TagIds);
