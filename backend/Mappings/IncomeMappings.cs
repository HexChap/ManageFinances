using backend.DTOs;
using backend.Models;

namespace backend.Mappings;

public static class IncomeMappings
{
    public static IncomeResponse ToResponse(this Income i) =>
        new(i.Id, i.Value, i.UserId, i.CreatedAt);
}
