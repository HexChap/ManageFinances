# Backend Rules

ASP.NET Core 10 REST API with PostgreSQL and EF Core.

---

## Project Structure

```
Controllers/     # Thin HTTP handlers only — no business logic
Services/        # Business logic, one class per domain concept
Data/            # DbContext only
Models/          # EF Core entities
DTOs/            # Request and response types (records)
```

Add a `Services/` layer before adding any logic beyond simple CRUD. Controllers call services; services own business logic and talk to the DB.

---

## C# Style

- Use `record` for all DTOs (request/response types)
- Use `readonly record struct` for value objects (IDs, Money, etc.)
- Enable and respect nullable reference types — no `!` suppression without a comment explaining why
- All async methods accept `CancellationToken ct = default` as the last parameter
- No `.Result`, `.Wait()`, or blocking calls on async code
- Pattern match with `switch` expressions over chains of `if`/`else if`
- Use `ArgumentNullException.ThrowIfNull()` (C# 11+) for guard clauses
- No AutoMapper or any reflection-based mapping — write explicit extension methods

```csharp
// ✅ DTO
public record CreateTransactionRequest(string Description, decimal Amount);
public record TransactionResponse(int Id, string Description, decimal Amount, DateTime CreatedAt);

// ✅ Explicit mapping
public static class TransactionMappings
{
    public static TransactionResponse ToResponse(this Transaction t) =>
        new(t.Id, t.Description, t.Amount, t.CreatedAt);
}
```

---

## EF Core

**NoTracking by default.** The `AppDbContext` constructor sets `QueryTrackingBehavior.NoTracking`. This means:

- For reads: works as normal
- For updates: you **must** call `dbContext.Entity.Update(entity)` or use `.AsTracking()` on the query — silent no-ops otherwise
- For deletes: attach a stub entity and call `Remove()`

```csharp
// ✅ Update pattern (NoTracking context)
var tx = await _db.Transactions.FirstOrDefaultAsync(t => t.Id == id, ct)
    ?? throw new NotFoundException($"Transaction {id} not found");
tx.Description = dto.Description;
_db.Transactions.Update(tx);
await _db.SaveChangesAsync(ct);

// ✅ Bulk update — never load entities just to update them
await _db.Transactions
    .Where(t => t.CreatedAt < cutoff)
    .ExecuteDeleteAsync(ct);
```

**Never edit migration files.** Only use the CLI:

```bash
dotnet ef migrations add <Name>
dotnet ef migrations remove
dotnet ef database update
```

**No N+1 queries.** Use `Include()` / `ThenInclude()` for navigation properties, or `SplitQuery` when loading multiple collections.

---

## Controllers

Controllers are thin. They:
1. Validate the route/body shape (model binding handles this)
2. Call a service method
3. Return an appropriate `IActionResult`

No DB access, no business logic, no mapping logic beyond calling `.ToResponse()`.

```csharp
[HttpPost]
public async Task<IActionResult> Create(
    CreateTransactionRequest request,
    CancellationToken ct)
{
    var result = await _transactionService.CreateAsync(request, ct);
    return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
}
```

Use these status codes consistently:
- `200 OK` — successful GET
- `201 Created` — successful POST with `Location` header
- `204 No Content` — successful DELETE or PUT with no body
- `404 Not Found` — resource does not exist
- `400 Bad Request` — invalid input (validation failure)

---

## Error Handling

Throw domain exceptions from services; catch and convert at a global middleware or `IExceptionHandler`, not in controllers.

```csharp
// Throw in service
throw new NotFoundException($"Transaction {id} not found");

// Global handler maps exception type → HTTP status
// Do NOT use try/catch in every controller action
```

Do not return `null` from service methods — throw or return a `Result<T>` type if the caller needs to distinguish success/failure without exceptions.

---

## Configuration

- Connection strings in `appsettings.json` / environment variables — never hardcoded
- Validate required config at startup using `IValidateOptions<T>`; fail fast on missing values rather than crashing mid-request

---

## Logging

Inject `ILogger<T>` via constructor — never use `Console.WriteLine` or `Debug.WriteLine` in production code.

Log at the right level:
- `LogInformation` — significant state changes (transaction created, migration applied)
- `LogWarning` — recoverable issues (resource not found, invalid input that passed validation)
- `LogError` — exceptions and failures that need attention

```csharp
// ✅
_logger.LogInformation("Transaction {Id} created for amount {Amount}", id, amount);
_logger.LogError(ex, "Failed to delete transaction {Id}", id);

// ❌
Console.WriteLine($"Created transaction {id}");
```

Do not log sensitive data (user input, amounts in financial contexts where regulations apply). Do not log on every read — only on writes and errors.

---

## Request Validation

Use DataAnnotations on request records. The `[ApiController]` attribute automatically returns `400 Bad Request` when model validation fails — no manual checks needed in controllers or services.

```csharp
public record CreateTransactionRequest(
    [Required, MaxLength(200)] string Description,
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be positive")] decimal Amount
);
```

Rules:
- Validate at the boundary (the request DTO) — not inside the service
- Do not repeat validation logic in services that is already covered by annotations
- For complex cross-field rules, implement `IValidatableObject` on the record

---

## Git Workflow

**Commit format:** `type: description` — lowercase, imperative mood

Valid types: `feat`, `fix`, `refactor`, `test`, `docs`

```
feat: add transaction filtering by date range
fix: return 404 when transaction not found
refactor: extract TransactionService from controller
```

**Before every commit:** run the test suite. Do not commit if tests are red.

```bash
dotnet test
```

**Never** add `Co-Authored-By` or any contributor lines to commit messages.

---

## What Not To Do

- Do not add logic to `Program.cs` beyond service registration and middleware pipeline
- Do not put business logic in controllers or `AppDbContext`
- Do not use `var` when the type isn't obvious from the right-hand side
- Do not return mutable collections from service methods — use `IReadOnlyList<T>`
- Do not create helper/utility classes for one-off operations
- Do not run `database.MigrateAsync()` in `Program.cs` in production — use a dedicated migration step or startup check
- Do not ignore EF Core warnings in output — treat them as bugs
