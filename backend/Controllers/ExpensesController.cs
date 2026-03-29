using System.Security.Claims;
using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
/// <summary>Handles HTTP requests for expense entries. All endpoints require authentication.</summary>
public class ExpensesController : ControllerBase
{
    private readonly ExpenseService _expenses;

    /// <param name="expenses">Expense business logic service.</param>
    public ExpensesController(ExpenseService expenses)
    {
        _expenses = expenses;
    }

    /// <summary>Extracts the authenticated user's ID from JWT claims.</summary>
    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Creates a new expense entry. Returns 201 with the created resource.</summary>
    [HttpPost]
    public async Task<IActionResult> Create(CreateExpenseRequest request, CancellationToken ct)
    {
        ExpenseResponse result = await _expenses.CreateAsync(request, GetUserId(), ct);
        return CreatedAtAction(nameof(GetByUser), null, result);
    }

    /// <summary>Returns expenses for the authenticated user. Optionally filtered by <paramref name="period"/>.</summary>
    [HttpGet]
    public async Task<IActionResult> GetByUser(
        [FromQuery] ExpensePeriod period = ExpensePeriod.All,
        CancellationToken ct = default)
    {
        IReadOnlyList<ExpenseResponse> result = await _expenses.GetByUserAsync(GetUserId(), period, ct);
        return Ok(result);
    }

    /// <summary>Updates an expense entry. Returns 404 if not found or not owned by the user.</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateExpenseRequest request, CancellationToken ct)
    {
        ExpenseResponse result = await _expenses.UpdateAsync(id, request, GetUserId(), ct);
        return Ok(result);
    }

    /// <summary>Deletes an expense entry. Returns 404 if not found.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _expenses.DeleteAsync(id, ct);
        return NoContent();
    }
}
