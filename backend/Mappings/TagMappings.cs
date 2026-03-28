using backend.DTOs;
using backend.Models;

namespace backend.Mappings;

public static class TagMappings
{
    public static TagResponse ToResponse(this Tag t) =>
        new(t.Id, t.Name, t.UserId);
}
