using System.ComponentModel.DataAnnotations;

namespace backend.DTOs;

public record CreateCategoryRequest(
    [Required, MaxLength(32)] string Name
);

public record UpdateCategoryRequest(
    [Required, MaxLength(32)] string Name
);

public record CategoryResponse(int Id, string Name, int? UserId, DateTime CreatedAt);
