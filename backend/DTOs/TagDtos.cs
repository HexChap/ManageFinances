using System.ComponentModel.DataAnnotations;

namespace backend.DTOs;

public record CreateTagRequest(
    [Required, MaxLength(32)] string Name,
    [Range(1, int.MaxValue)] int UserId
);

public record TagResponse(int Id, string Name, int UserId);
