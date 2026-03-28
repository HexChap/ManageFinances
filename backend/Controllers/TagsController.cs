using System.Security.Claims;
using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly TagService _tags;

    public TagsController(TagService tags)
    {
        _tags = tags;
    }

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<IActionResult> Create(CreateTagRequest request, CancellationToken ct)
    {
        TagResponse result = await _tags.CreateAsync(request, GetUserId(), ct);
        return CreatedAtAction(nameof(GetByUser), null, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetByUser(CancellationToken ct)
    {
        IReadOnlyList<TagResponse> result = await _tags.GetByUserAsync(GetUserId(), ct);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _tags.DeleteAsync(id, ct);
        return NoContent();
    }
}
