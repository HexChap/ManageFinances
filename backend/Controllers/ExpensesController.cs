using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExpensesController : ControllerBase
{
    private readonly ExpenseService _expenses;

    public ExpensesController(ExpenseService expenses)
    {
        _expenses = expenses;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateExpenseRequest request, CancellationToken ct)
    {
        ExpenseResponse result = await _expenses.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetByUser), new { userId = result.UserId }, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetByUser(
        [FromQuery] int userId,
        [FromQuery] ExpensePeriod period = ExpensePeriod.All,
        CancellationToken ct = default)
    {
        IReadOnlyList<ExpenseResponse> result = await _expenses.GetByUserAsync(userId, period, ct);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _expenses.DeleteAsync(id, ct);
        return NoContent();
    }
}
