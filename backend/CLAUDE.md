# Backend Rules

ASP.NET Core 10 REST API with PostgreSQL and EF Core.

---

## Project Structure

```
Controllers/     # Thin HTTP handlers only — no business logic
Services/        # Business logic, one class per domain concept
                 # Tags have a many-to-many with Expenses (ExpenseTag join table)
Data/            # DbContext only
Models/          # EF Core entities
DTOs/            # Request and response types (records)
Mappings/        # Explicit .ToResponse() extension methods per entity
Exceptions/      # NotFoundException + GlobalExceptionHandler
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
public record CreateExpenseRequest(int CategoryId, decimal Value, int UserId);
public record ExpenseResponse(int Id, int CategoryId, decimal Value, int UserId, DateTime CreatedAt);

// ✅ Explicit mapping
public static class ExpenseMappings
{
    public static ExpenseResponse ToResponse(this Expense e) =>
        new(e.Id, e.CategoryId, e.Value, e.UserId, e.CreatedAt);
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
var expense = await _db.Expenses.AsTracking().FirstOrDefaultAsync(e => e.Id == id, ct)
    ?? throw new NotFoundException($"Expense {id} not found");
expense.Value = dto.Value;
_db.Expenses.Update(expense);
await _db.SaveChangesAsync(ct);

// ✅ Single-entity delete
var entity = await _db.Expenses.FindAsync([id], ct)
    ?? throw new NotFoundException($"Expense {id} not found");
_db.Expenses.Remove(entity);
await _db.SaveChangesAsync(ct);

// ✅ Bulk delete
await _db.Expenses
    .Where(e => e.CreatedAt < cutoff)
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
    var result = await _expenseService.CreateAsync(request, ct);
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
_logger.LogInformation("Expense {Id} created for user {UserId}", id, userId);
_logger.LogError(ex, "Failed to delete expense {Id}", id);

// ❌
Console.WriteLine($"Created expense {id}");
```

Do not log sensitive data (user input, amounts in financial contexts where regulations apply). Do not log on every read — only on writes and errors.

---

## Request Validation

Use DataAnnotations on request records. The `[ApiController]` attribute automatically returns `400 Bad Request` when model validation fails — no manual checks needed in controllers or services.

```csharp
public record CreateExpenseRequest(
    [Range(1, int.MaxValue)] int CategoryId,
    [Range(0.01, double.MaxValue, ErrorMessage = "Value must be positive")] decimal Value,
    [Range(1, int.MaxValue)] int UserId
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
feat: add expense filtering by date range
fix: return 404 when expense not found
refactor: extract ExpenseService from controller
```

**Before every commit:** run the test suite. Do not commit if tests are red.

```bash
dotnet test ManageFinances.slnx
```

**Never** add `Co-Authored-By` or any contributor lines to commit messages.

---

## Testing

Test project lives at `backend.Tests/` (inside `backend/`). It references `backend.csproj` directly.

`backend.csproj` excludes `backend.Tests/**` via `<Compile Remove>` — required because the test project is a subdirectory of the API project.

`ExecuteDeleteAsync` is **not** supported by the InMemory provider — use `FindAsync + Remove` for single-entity deletes so tests work without a real DB.

---

## Known Warnings

The test project produces an MSB3277 warning about conflicting `Microsoft.EntityFrameworkCore.Relational` versions. This is a pre-existing package version skew between `backend.csproj` and `backend.Tests.csproj` — not caused by new code. The build succeeds; ignore it.

---

## What Not To Do

- Do not add logic to `Program.cs` beyond service registration and middleware pipeline
- Do not put business logic in controllers or `AppDbContext`
- Do not use `var` when the type isn't obvious from the right-hand side
- Do not return mutable collections from service methods — use `IReadOnlyList<T>`
- Do not create helper/utility classes for one-off operations
- Do not run `database.MigrateAsync()` in `Program.cs` in production — use a dedicated migration step or startup check
- Do not ignore EF Core warnings in output — treat them as bugs
