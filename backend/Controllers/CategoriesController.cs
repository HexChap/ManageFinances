using System.Security.Claims;
using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
/// <summary>Handles HTTP requests for expense categories. All endpoints require authentication.</summary>
public class CategoriesController : ControllerBase
{
    private readonly CategoryService _categories;

    /// <param name="categories">Category business logic service.</param>
    public CategoriesController(CategoryService categories)
    {
        _categories = categories;
    }

    /// <summary>Extracts the authenticated user's ID from JWT claims.</summary>
    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Creates a new user-owned category. Returns 201 with the created resource.</summary>
    [HttpPost]
    public async Task<IActionResult> Create(CreateCategoryRequest request, CancellationToken ct)
    {
        CategoryResponse result = await _categories.CreateAsync(request, GetUserId(), ct);
        return CreatedAtAction(nameof(GetByUser), null, result);
    }

    /// <summary>Returns all categories visible to the authenticated user (own + global).</summary>
    [HttpGet]
    public async Task<IActionResult> GetByUser(CancellationToken ct)
    {
        IReadOnlyList<CategoryResponse> result = await _categories.GetByUserAsync(GetUserId(), ct);
        return Ok(result);
    }

    /// <summary>Updates a category name. Returns 404 if not found, global, or not owned by the user.</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateCategoryRequest request, CancellationToken ct)
    {
        CategoryResponse result = await _categories.UpdateAsync(id, request, GetUserId(), ct);
        return Ok(result);
    }

    /// <summary>Deletes a category. Returns 404 if not found.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _categories.DeleteAsync(id, ct);
        return NoContent();
    }
}
