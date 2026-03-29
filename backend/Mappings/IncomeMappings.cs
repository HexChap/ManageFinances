using backend.DTOs;
using backend.Models;

namespace backend.Mappings;

/// <summary>Explicit mapping extensions for <see cref="Income"/> entities.</summary>
public static class IncomeMappings
{
    /// <summary>Maps an <see cref="Income"/> entity to an <see cref="IncomeResponse"/> DTO.</summary>
    public static IncomeResponse ToResponse(this Income i) =>
        new(i.Id, i.Value, i.UserId, i.CreatedAt);
}
