# Manage Finances

Full-stack personal finance manager — track expenses, incomes, and categories.

## Stack

| Layer | Technology |
|---|---|
| Frontend | React 19, TypeScript, Tailwind CSS v4, shadcn/ui |
| Backend | ASP.NET Core 10, EF Core, PostgreSQL |
| Infrastructure | Docker Compose, Nginx |

## Domain

- **Users** — each user has an optional timezone
- **Categories** — global (no owner) or user-specific; expenses belong to a category
- **Expenses** — linked to a category and a user; filterable by today / current month / all time
- **Incomes** — linked to a user

## Running Locally

```bash
docker compose up --build
```

- Frontend: http://localhost
- API: http://localhost/api

## API Endpoints

### Users
```
POST   /api/users
GET    /api/users/{id}
PUT    /api/users/{id}
```

### Categories
```
POST   /api/categories
GET    /api/categories?userId={id}
DELETE /api/categories/{id}
```

### Expenses
```
POST   /api/expenses
GET    /api/expenses?userId={id}&period=All|Today|Month
DELETE /api/expenses/{id}
```

### Incomes
```
POST   /api/incomes
GET    /api/incomes?userId={id}
DELETE /api/incomes/{id}
```

## Development

See [`frontend/README.md`](frontend/README.md) for frontend setup.
See [`backend/CLAUDE.md`](backend/CLAUDE.md) for backend coding conventions.

After changing the backend schema, regenerate and apply the migration:

```bash
cd backend
dotnet ef migrations add <Name>
dotnet ef database update
```
