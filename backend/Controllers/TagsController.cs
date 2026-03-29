using System.Security.Claims;
using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
/// <summary>Handles HTTP requests for user-defined tags. All endpoints require authentication.</summary>
public class TagsController : ControllerBase
{
    private readonly TagService _tags;

    /// <param name="tags">Tag business logic service.</param>
    public TagsController(TagService tags)
    {
        _tags = tags;
    }

    /// <summary>Extracts the authenticated user's ID from JWT claims.</summary>
    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Creates a new tag. Returns 201 with the created resource.</summary>
    [HttpPost]
    public async Task<IActionResult> Create(CreateTagRequest request, CancellationToken ct)
    {
        TagResponse result = await _tags.CreateAsync(request, GetUserId(), ct);
        return CreatedAtAction(nameof(GetByUser), null, result);
    }

    /// <summary>Returns all tags belonging to the authenticated user.</summary>
    [HttpGet]
    public async Task<IActionResult> GetByUser(CancellationToken ct)
    {
        IReadOnlyList<TagResponse> result = await _tags.GetByUserAsync(GetUserId(), ct);
        return Ok(result);
    }

    /// <summary>Updates a tag name. Returns 404 if not found or not owned by the user.</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateTagRequest request, CancellationToken ct)
    {
        TagResponse result = await _tags.UpdateAsync(id, request, GetUserId(), ct);
        return Ok(result);
    }

    /// <summary>Deletes a tag. Returns 404 if not found.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _tags.DeleteAsync(id, ct);
        return NoContent();
    }
}
