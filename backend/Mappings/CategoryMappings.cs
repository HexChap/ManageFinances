using backend.DTOs;
using backend.Models;

namespace backend.Mappings;

/// <summary>Explicit mapping extensions for <see cref="Category"/> entities.</summary>
public static class CategoryMappings
{
    /// <summary>Maps a <see cref="Category"/> entity to a <see cref="CategoryResponse"/> DTO.</summary>
    public static CategoryResponse ToResponse(this Category c) =>
        new(c.Id, c.Name, c.UserId, c.CreatedAt);
}
