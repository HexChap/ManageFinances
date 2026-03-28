using System.Security.Claims;
using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ExpensesController : ControllerBase
{
    private readonly ExpenseService _expenses;

    public ExpensesController(ExpenseService expenses)
    {
        _expenses = expenses;
    }

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<IActionResult> Create(CreateExpenseRequest request, CancellationToken ct)
    {
        ExpenseResponse result = await _expenses.CreateAsync(request, GetUserId(), ct);
        return CreatedAtAction(nameof(GetByUser), null, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetByUser(
        [FromQuery] ExpensePeriod period = ExpensePeriod.All,
        CancellationToken ct = default)
    {
        IReadOnlyList<ExpenseResponse> result = await _expenses.GetByUserAsync(GetUserId(), period, ct);
        return Ok(result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateExpenseRequest request, CancellationToken ct)
    {
        ExpenseResponse result = await _expenses.UpdateAsync(id, request, GetUserId(), ct);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _expenses.DeleteAsync(id, ct);
        return NoContent();
    }
}
