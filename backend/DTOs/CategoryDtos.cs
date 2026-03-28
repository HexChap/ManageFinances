using System.ComponentModel.DataAnnotations;

namespace backend.DTOs;

public record CreateCategoryRequest(
    [Required, MaxLength(32)] string Name,
    int? UserId
);

public record CategoryResponse(int Id, string Name, int? UserId, DateTime CreatedAt);
