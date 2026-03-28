using System.ComponentModel.DataAnnotations;

namespace backend.DTOs;

public record CreateIncomeRequest(
    [Range(0.01, double.MaxValue, ErrorMessage = "Value must be positive")] decimal Value,
    [Range(1, int.MaxValue)] int UserId
);

public record IncomeResponse(int Id, decimal Value, int UserId, DateTime CreatedAt);
