using System.Security.Claims;
using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
/// <summary>Handles HTTP requests for income entries. All endpoints require authentication.</summary>
public class IncomesController : ControllerBase
{
    private readonly IncomeService _incomes;

    /// <param name="incomes">Income business logic service.</param>
    public IncomesController(IncomeService incomes)
    {
        _incomes = incomes;
    }

    /// <summary>Extracts the authenticated user's ID from JWT claims.</summary>
    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Creates a new income entry. Returns 201 with the created resource.</summary>
    [HttpPost]
    public async Task<IActionResult> Create(CreateIncomeRequest request, CancellationToken ct)
    {
        IncomeResponse result = await _incomes.CreateAsync(request, GetUserId(), ct);
        return CreatedAtAction(nameof(GetByUser), null, result);
    }

    /// <summary>Returns all income entries for the authenticated user.</summary>
    [HttpGet]
    public async Task<IActionResult> GetByUser(CancellationToken ct)
    {
        IReadOnlyList<IncomeResponse> result = await _incomes.GetByUserAsync(GetUserId(), ct);
        return Ok(result);
    }

    /// <summary>Updates an income entry. Returns 404 if not found or not owned by the user.</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateIncomeRequest request, CancellationToken ct)
    {
        IncomeResponse result = await _incomes.UpdateAsync(id, request, GetUserId(), ct);
        return Ok(result);
    }

    /// <summary>Deletes an income entry. Returns 404 if not found.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _incomes.DeleteAsync(id, ct);
        return NoContent();
    }
}
