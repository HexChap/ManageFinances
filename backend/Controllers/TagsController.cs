using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly TagService _tags;

    public TagsController(TagService tags)
    {
        _tags = tags;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTagRequest request, CancellationToken ct)
    {
        TagResponse result = await _tags.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetByUser), new { userId = result.UserId }, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetByUser([FromQuery] int userId, CancellationToken ct)
    {
        IReadOnlyList<TagResponse> result = await _tags.GetByUserAsync(userId, ct);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _tags.DeleteAsync(id, ct);
        return NoContent();
    }
}
