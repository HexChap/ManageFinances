using System.ComponentModel.DataAnnotations;

namespace backend.DTOs;

public record CreateIncomeRequest(
    [Range(0.01, double.MaxValue, ErrorMessage = "Value must be positive")] decimal Value
);

public record IncomeResponse(int Id, decimal Value, int UserId, DateTime CreatedAt);
