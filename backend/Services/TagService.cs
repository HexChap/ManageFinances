using backend.Data;
using backend.DTOs;
using backend.Exceptions;
using backend.Mappings;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

/// <summary>
/// Business logic for managing user-defined tags.
/// Tags are user-scoped: two users may have tags with the same name independently.
/// </summary>
public class TagService
{
    private readonly AppDbContext _db;
    private readonly ILogger<TagService> _logger;

    /// <param name="db">Database context.</param>
    /// <param name="logger">Logger for write operation audit trail.</param>
    public TagService(AppDbContext db, ILogger<TagService> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>Creates a new tag for the given user.</summary>
    /// <param name="request">Tag details.</param>
    /// <param name="userId">ID of the authenticated user.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created tag as a response DTO.</returns>
    public async Task<TagResponse> CreateAsync(CreateTagRequest request, int userId, CancellationToken ct = default)
    {
        var tag = new Tag { Name = request.Name, UserId = userId };
        _db.Tags.Add(tag);
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Tag {Id} created for user {UserId}", tag.Id, tag.UserId);
        return tag.ToResponse();
    }

    /// <summary>Returns all tags belonging to the given user.</summary>
    /// <param name="userId">ID of the authenticated user.</param>
    /// <param name="ct">Cancellation token.</param>
    public async Task<IReadOnlyList<TagResponse>> GetByUserAsync(int userId, CancellationToken ct = default)
    {
        List<Tag> tags = await _db.Tags
            .Where(t => t.UserId == userId)
            .ToListAsync(ct);
        return tags.Select(t => t.ToResponse()).ToList();
    }

    /// <summary>Updates the name of an existing tag.</summary>
    /// <param name="id">ID of the tag to update.</param>
    /// <param name="request">New tag details.</param>
    /// <param name="userId">ID of the authenticated user. Must match the tag owner.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated tag as a response DTO.</returns>
    /// <exception cref="NotFoundException">Thrown when the tag does not exist or belongs to a different user.</exception>
    public async Task<TagResponse> UpdateAsync(int id, UpdateTagRequest request, int userId, CancellationToken ct = default)
    {
        Tag tag = await _db.Tags.AsTracking().FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId, ct)
            ?? throw new NotFoundException($"Tag {id} not found");
        tag.Name = request.Name;
        _db.Tags.Update(tag);
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Tag {Id} updated", id);
        return tag.ToResponse();
    }

    /// <summary>Deletes a tag by ID.</summary>
    /// <param name="id">ID of the tag to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <exception cref="NotFoundException">Thrown when the tag does not exist.</exception>
    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        Tag tag = await _db.Tags.FindAsync([id], ct)
            ?? throw new NotFoundException($"Tag {id} not found");
        _db.Tags.Remove(tag);
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Tag {Id} deleted", id);
    }
}
