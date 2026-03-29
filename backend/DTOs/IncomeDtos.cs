using System.ComponentModel.DataAnnotations;

namespace backend.DTOs;

/// <summary>Request body for creating a new income entry.</summary>
public record CreateIncomeRequest(
    /// <summary>Monetary value. Must be greater than zero.</summary>
    [Range(0.01, double.MaxValue, ErrorMessage = "Value must be positive")] decimal Value
);

/// <summary>Request body for updating an existing income entry.</summary>
public record UpdateIncomeRequest(
    /// <summary>New monetary value. Must be greater than zero.</summary>
    [Range(0.01, double.MaxValue, ErrorMessage = "Value must be positive")] decimal Value
);

/// <summary>Read model returned for income entries.</summary>
public record IncomeResponse(int Id, decimal Value, int UserId, DateTime CreatedAt);
