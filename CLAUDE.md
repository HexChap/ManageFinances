# Manage Finances

## Local Environment

Spin up with `docker compose up --build`.

The backend runs on port **8080 inside Docker only** — not mapped to the host.
Access the API through the Nginx proxy: `http://localhost/api`

## Structure

```
backend/              # ASP.NET Core API + solution
  ManageFinances.slnx
  backend.Tests/      # xUnit tests (separate project, inside backend/)
frontend/             # React + Vite SPA
```

## Commands

```bash
dotnet test backend/ManageFinances.slnx
```

```bash
# Frontend
cd frontend && bun run dev         # dev server (proxied via Nginx in Docker)
bun run typecheck && bun run lint  # run before committing
```

```bash
# EF Core migrations
cd backend
dotnet ef migrations add <Name>
dotnet ef database update
```

## See Also

- `backend/CLAUDE.md` — ASP.NET Core coding rules and EF Core patterns
- `frontend/CLAUDE.md` — React/TypeScript rules, axios patterns, hook conventions
