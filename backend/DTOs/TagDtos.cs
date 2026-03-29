using System.ComponentModel.DataAnnotations;

namespace backend.DTOs;

/// <summary>Request body for creating a new tag.</summary>
public record CreateTagRequest(
    /// <summary>Display name of the tag. Maximum 32 characters.</summary>
    [Required, MaxLength(32)] string Name
);

/// <summary>Request body for updating an existing tag.</summary>
public record UpdateTagRequest(
    /// <summary>New display name. Maximum 32 characters.</summary>
    [Required, MaxLength(32)] string Name
);

/// <summary>Read model returned for tag entries.</summary>
public record TagResponse(int Id, string Name, int UserId);
