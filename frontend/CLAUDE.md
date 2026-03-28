# Frontend Rules

React 19 + TypeScript SPA, Vite, Tailwind CSS v4, shadcn/ui (Radix), React Router v7.

---

## Project Structure

```
src/
  pages/          # Route-level components — one file per route
  components/     # Shared/reusable UI components
    ui/           # shadcn/ui primitives — do not edit directly
  hooks/          # Custom hooks (useExpenses, useAuth, etc.)
  services/       # API call functions — no fetch() outside this layer
  types/          # Shared TypeScript types and interfaces
  lib/            # Pure utilities (no React, no side effects)
```

Keep pages thin: they compose components, call hooks, and handle layout — no business logic or fetch calls directly in page files.

---

## TypeScript

- Strict mode is on — no `any`, no `as any`, no `@ts-ignore`
- Prefer `interface` for object shapes, `type` for unions and aliases
- Always type component props explicitly — no implicit `{}` or inlined inline objects without a name
- Never cast with `as X` to silence a type error — fix the root cause

```tsx
// ✅
interface ExpenseCardProps {
  expense: Expense;
  onDelete: (id: number) => void;
}

// ❌
const ExpenseCard = ({ expense, onDelete }: any) => { ... }
```

---

## Components

- Functional components only — no class components
- One component per file; file name matches the exported component name (`ExpenseCard.tsx` exports `ExpenseCard`)
- Use named exports everywhere — no default exports except in `pages/`
- Keep components focused: if a component has more than ~100 lines or does two distinct things, split it

```tsx
// ✅ named export for shared component
export function ExpenseCard({ expense, onDelete }: ExpenseCardProps) { ... }

// ✅ default export only for pages
export default function Finances() { ... }
```

---

## API Layer

All HTTP calls live in `src/services/`. Pages and components never use `axios` directly.

- One file per domain: `expenseService.ts`, `incomeService.ts`, `authService.ts`, etc.
- All requests go through a central `api.ts` axios instance that sets the base URL and default headers
- Return typed response objects — no `any` return types
- Axios throws on non-2xx by default — do not catch and return `null`

```ts
// src/services/api.ts
import axios from 'axios';

const api = axios.create({ baseURL: '/api' });
export default api;

// src/services/expenseService.ts
import api from './api';

export async function getExpenses(): Promise<Expense[]> {
  const { data } = await api.get<Expense[]>('/expenses');
  return data;
}
```

---

## State Management

No global state library. Use:

- `useState` / `useReducer` for local UI state
- Custom hooks in `src/hooks/` for data-fetching and shared logic
- React Context only for genuinely cross-cutting state (auth, theme) — one context per concern

Do not lift state higher than necessary. Do not put server data in a Context — keep it close to the component that needs it.

---

## Custom Hooks

Extract data-fetching into hooks. A hook owns loading, error, and data state for one resource.

```ts
// src/hooks/useExpenses.ts
export function useExpenses() {
  const [expenses, setExpenses] = useState<Expense[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    getExpenses()
      .then(setExpenses)
      .catch((e) => setError(e.message))
      .finally(() => setLoading(false));
  }, []);

  return { expenses, loading, error };
}
```

---

## Styling

- Tailwind utility classes only — no inline `style={{}}` unless driven by a dynamic value that Tailwind cannot express
- Use `cn()` from `src/lib/utils.ts` for conditional class merging
- Do not add custom CSS unless absolutely unavoidable — prefer Tailwind variants
- Follow the existing shadcn/ui component patterns; do not override their internal styles by targeting internal class names

---

## Forms

Use controlled inputs. Validate on submit, not on every keystroke unless UX requires it. Show validation errors inline, not via `alert()`.

No form library unless the complexity clearly warrants it.

---

## Routing

Routes are defined in `App.tsx`. Add new routes there.

- Protected routes (require auth) get their own wrapper component — do not duplicate auth checks in every page
- Use `useNavigate` for programmatic navigation — never `window.location.href`

---

## Error Handling

- Catch errors at the hook or component that owns the operation
- Show user-facing error messages in the UI — never `console.error` as the only response
- Do not swallow errors silently

---

## Commands

Package manager is **bun**.

```bash
cd frontend
bun run dev        # dev server
bun run typecheck  # type-check without building
bun run lint       # ESLint
bun run format     # Prettier
bun run build      # production build
bun add <pkg>      # install a dependency
```

**Before every commit:** run `bun run typecheck && bun run lint`. Do not commit if either fails.

---

## What Not To Do

- Do not fetch data directly in page or component bodies — use hooks
- Do not import `axios` directly in components or pages — always go through `services/api.ts`
- Do not use `useEffect` for things that aren't side effects (derive values with `useMemo` instead)
- Do not edit files under `components/ui/` — regenerate them with `shadcn` CLI if an update is needed
- Do not hardcode API URLs — always use the base client in `services/api.ts`
- Do not store tokens or sensitive data in `localStorage` — use `httpOnly` cookies (auth is server-managed)
- Do not add dependencies without checking if something already installed covers the use case
