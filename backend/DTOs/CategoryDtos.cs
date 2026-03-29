using System.ComponentModel.DataAnnotations;

namespace backend.DTOs;

/// <summary>Request body for creating a new category.</summary>
public record CreateCategoryRequest(
    /// <summary>Display name of the category. Maximum 32 characters.</summary>
    [Required, MaxLength(32)] string Name
);

/// <summary>Request body for updating an existing category.</summary>
public record UpdateCategoryRequest(
    /// <summary>New display name. Maximum 32 characters.</summary>
    [Required, MaxLength(32)] string Name
);

/// <summary>Read model returned for category entries.</summary>
public record CategoryResponse(int Id, string Name, int? UserId, DateTime CreatedAt);
