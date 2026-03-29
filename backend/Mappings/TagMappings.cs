using backend.DTOs;
using backend.Models;

namespace backend.Mappings;

/// <summary>Explicit mapping extensions for <see cref="Tag"/> entities.</summary>
public static class TagMappings
{
    /// <summary>Maps a <see cref="Tag"/> entity to a <see cref="TagResponse"/> DTO.</summary>
    public static TagResponse ToResponse(this Tag t) =>
        new(t.Id, t.Name, t.UserId);
}
