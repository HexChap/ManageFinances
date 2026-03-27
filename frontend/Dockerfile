# syntax=docker/dockerfile:1.4

# ---- deps stage ----
FROM oven/bun:1-alpine AS deps
WORKDIR /app

COPY package.json bun.lock ./

RUN --mount=type=cache,target=/root/.bun/install/cache \
    bun install --frozen-lockfile

# ---- build stage ----
FROM oven/bun:1-alpine AS builder
WORKDIR /app

COPY --from=deps /app/node_modules ./node_modules
COPY . .

RUN --mount=type=cache,target=/root/.bun/install/cache \
    bun run build

# ---- production stage ----
FROM nginx:alpine AS production

COPY --from=builder /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
