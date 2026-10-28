# Build stage: production Angular build (environment.ts, no API mock).
FROM node:26-alpine AS build
WORKDIR /app

COPY package.json package-lock.json ./
RUN npm ci

COPY . .
RUN npx ng build

# Runtime stage: nginx serves the SPA and proxies /api and /uploads to the API container.
FROM nginx:stable-alpine
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/client/browser /usr/share/nginx/html
