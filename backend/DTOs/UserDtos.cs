using System.ComponentModel.DataAnnotations;

namespace backend.DTOs;

public record CreateUserRequest(
    [MaxLength(48)] string? Timezone
);

public record UpdateUserRequest(
    [MaxLength(48)] string? Timezone
);

public record UserResponse(int Id, string? Timezone, DateTime CreatedAt);
