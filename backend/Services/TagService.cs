using backend.Data;
using backend.DTOs;
using backend.Exceptions;
using backend.Mappings;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class TagService
{
    private readonly AppDbContext _db;
    private readonly ILogger<TagService> _logger;

    public TagService(AppDbContext db, ILogger<TagService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<TagResponse> CreateAsync(CreateTagRequest request, int userId, CancellationToken ct = default)
    {
        var tag = new Tag { Name = request.Name, UserId = userId };
        _db.Tags.Add(tag);
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Tag {Id} created for user {UserId}", tag.Id, tag.UserId);
        return tag.ToResponse();
    }

    public async Task<IReadOnlyList<TagResponse>> GetByUserAsync(int userId, CancellationToken ct = default)
    {
        List<Tag> tags = await _db.Tags
            .Where(t => t.UserId == userId)
            .ToListAsync(ct);
        return tags.Select(t => t.ToResponse()).ToList();
    }

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

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        Tag tag = await _db.Tags.FindAsync([id], ct)
            ?? throw new NotFoundException($"Tag {id} not found");
        _db.Tags.Remove(tag);
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Tag {Id} deleted", id);
    }
}
