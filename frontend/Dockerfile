FROM node:20 AS build
RUN corepack enable && corepack prepare pnpm@latest --activate
WORKDIR /app
COPY package*.json pnpm-lock.yaml ./
RUN pnpm install --frozen-lockfile
COPY . .
RUN pnpm run build

FROM joseluisq/static-web-server:2-alpine
COPY --from=build /app/dist /public
ENTRYPOINT [ "static-web-server" ]