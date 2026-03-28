# Manage Finances — Frontend

React + TypeScript SPA for tracking personal expenses and incomes.

## Stack

- **React 19** with React Router v7
- **TypeScript 5.9**
- **Tailwind CSS v4** + **shadcn/ui** (Radix UI)
- **Vite 7**
- **Bun** (package manager)

## Pages

| Route | Description |
|---|---|
| `/` | Landing page |
| `/login` | Login form |
| `/signup` | Sign-up form |
| `/finances` | Main finance manager (expenses, incomes, categories) |

## Getting Started

```bash
bun install
bun run dev        # dev server at http://localhost:5173
bun run build      # production build
bun run typecheck  # type-check without emitting
bun run lint       # ESLint
bun run format     # Prettier
```

## Adding shadcn/ui components

```bash
npx shadcn@latest add <component>
# e.g. npx shadcn@latest add button
```

Components are placed in `src/components/ui/`.

## Project Structure

```
src/
  components/
    ui/          # shadcn/ui primitives
    *.tsx        # shared components (forms, layout)
  pages/         # one file per route
  main.tsx       # app entry point
```

## Environment

In production the frontend is served by Nginx and proxies `/api` to the backend.
For local dev without Docker, point requests at `http://localhost:8080/api`.
