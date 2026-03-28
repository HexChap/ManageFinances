using backend.DTOs;
using backend.Models;

namespace backend.Mappings;

public static class UserMappings
{
    public static UserResponse ToResponse(this User u) =>
        new(u.Id, u.Timezone, u.CreatedAt);
}
