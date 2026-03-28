using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly CategoryService _categories;

    public CategoriesController(CategoryService categories)
    {
        _categories = categories;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCategoryRequest request, CancellationToken ct)
    {
        CategoryResponse result = await _categories.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetByUser), new { userId = result.UserId }, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetByUser([FromQuery] int userId, CancellationToken ct)
    {
        IReadOnlyList<CategoryResponse> result = await _categories.GetByUserAsync(userId, ct);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _categories.DeleteAsync(id, ct);
        return NoContent();
    }
}
