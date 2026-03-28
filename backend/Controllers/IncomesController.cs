using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IncomesController : ControllerBase
{
    private readonly IncomeService _incomes;

    public IncomesController(IncomeService incomes)
    {
        _incomes = incomes;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateIncomeRequest request, CancellationToken ct)
    {
        IncomeResponse result = await _incomes.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetByUser), new { userId = result.UserId }, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetByUser([FromQuery] int userId, CancellationToken ct)
    {
        IReadOnlyList<IncomeResponse> result = await _incomes.GetByUserAsync(userId, ct);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _incomes.DeleteAsync(id, ct);
        return NoContent();
    }
}
