using System.ComponentModel.DataAnnotations;

namespace backend.DTOs;

public record CreateTagRequest(
    [Required, MaxLength(32)] string Name
);

public record UpdateTagRequest(
    [Required, MaxLength(32)] string Name
);

public record TagResponse(int Id, string Name, int UserId);
