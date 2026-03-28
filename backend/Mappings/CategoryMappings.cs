using backend.DTOs;
using backend.Models;

namespace backend.Mappings;

public static class CategoryMappings
{
    public static CategoryResponse ToResponse(this Category c) =>
        new(c.Id, c.Name, c.UserId, c.CreatedAt);
}
