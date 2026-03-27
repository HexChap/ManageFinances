using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;

namespace backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TransactionsController : ControllerBase
{
    private readonly AppDbContext _context;

    public TransactionsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Transactions (Read All)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
    {
        return await _context.Transactions.ToListAsync();
    }

    // GET: api/Transactions/5 (Read One)
    [HttpGet("{id}")]
    public async Task<ActionResult<Transaction>> GetTransaction(int id)
    {
        var transaction = await _context.Transactions.FindAsync(id);
        if (transaction == null) return NotFound();
        return transaction;
    }

    // POST: api/Transactions (Create)
    [HttpPost]
    public async Task<ActionResult<Transaction>> PostTransaction(Transaction transaction)
    {
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
    }

    // PUT: api/Transactions/5 (Update)
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTransaction(int id, Transaction transaction)
    {
        if (id != transaction.Id) return BadRequest();

        _context.Entry(transaction).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Transactions.Any(e => e.Id == id)) return NotFound();
            throw;
        }

        return NoContent();
    }

    // DELETE: api/Transactions/5 (Delete)
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTransaction(int id)
    {
        var transaction = await _context.Transactions.FindAsync(id);
        if (transaction == null) return NotFound();

        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
