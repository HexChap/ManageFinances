using System.Security.Claims;
using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class IncomesController : ControllerBase
{
    private readonly IncomeService _incomes;

    public IncomesController(IncomeService incomes)
    {
        _incomes = incomes;
    }

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<IActionResult> Create(CreateIncomeRequest request, CancellationToken ct)
    {
        IncomeResponse result = await _incomes.CreateAsync(request, GetUserId(), ct);
        return CreatedAtAction(nameof(GetByUser), null, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetByUser(CancellationToken ct)
    {
        IReadOnlyList<IncomeResponse> result = await _incomes.GetByUserAsync(GetUserId(), ct);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _incomes.DeleteAsync(id, ct);
        return NoContent();
    }
}
