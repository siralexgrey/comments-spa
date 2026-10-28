# Comments SPA

A single-page application for threaded comments: root comments in a sortable, paginated table, replies of any depth, a small HTML toolbar with live preview, CAPTCHA, and image/text attachments.

**Live demo:** https://comments-spa.runasp.net

## Features

- Form with client- and server-side validation: user name (Latin letters and digits), e-mail, optional home page URL, CAPTCHA, text.
- Text allows only `<a href="" title="">`, `<code>`, `<i>`, `<strong>`; it must be valid XHTML (unclosed tags are rejected). Toolbar buttons `[i] [strong] [code] [a]` and a preview without page reload.
- Replies to any comment, any depth.
- Root comments in a table: sorting by user name, e-mail and date in both directions, newest first by default, 25 per page.
- Attachments: JPG, GIF, PNG (resized proportionally to fit 320x240, animated GIFs keep all frames) or TXT up to 100 KB, opened in a lightbox.
- Protection against XSS (whitelist sanitizer, only `http`/`https` links) and SQL injection (EF Core parameters, whitelisted sort columns).

## Tech stack

| Layer | Technology |
|---|---|
| Backend | .NET 10, ASP.NET Core Web API, EF Core 10, SQL Server, FluentValidation, HtmlSanitizer, Magick.NET |
| Frontend | Angular 22, Angular Material, Tailwind CSS |
| Infrastructure | Docker Compose (SQL Server, API, nginx) |

The backend follows a layered layout: `Comments.Domain` → `Comments.Application` → `Comments.Infrastructure` / `Comments.Api`. The REST API is described in [docs/api-contract.md](docs/api-contract.md).

## Run with Docker

Requirements: Docker with Docker Compose. On Apple Silicon, enable "Use Rosetta for x86/amd64 emulation" in Docker Desktop: the SQL Server image is x86-only.

```bash
git clone https://github.com/siralexgrey/comments-spa.git
cd comments-spa
cp .env.example .env
```

Set `DB_PASSWORD` in `.env`. SQL Server requires at least 8 characters from three of these groups: uppercase, lowercase, digits, symbols. Do not use `;` (it breaks the connection string). `SFTP_TARGET` is only for deployment and can stay empty.

```bash
docker compose up --build
```

Open http://localhost:8080. The database schema is created automatically on the first start (EF Core migrations run when the API starts).

| Service | Purpose | Port |
|---|---|---|
| `client` | nginx: serves the Angular app, proxies `/api` and `/uploads` to the API | 8080 |
| `api` | ASP.NET Core API | internal only |
| `db` | SQL Server 2022 | `127.0.0.1:14330` (for local development) |

Uploaded files are stored in the `uploads-data` volume, the database in `db-data`. `docker compose down -v` removes both.

## Local development

Requirements: .NET SDK 10, Node.js 22.22+, 24.15+ or 26+ (required by Angular 22), Docker (for the database).

```bash
docker compose up -d db

cd server
dotnet user-secrets set "ConnectionStrings:Default" "Server=localhost,14330;Database=Comments;User Id=sa;Password=<DB_PASSWORD>;TrustServerCertificate=True" --project src/Comments.Api
dotnet run --project src/Comments.Api --launch-profile http
```

The API listens on http://localhost:5062, Swagger UI is at its root.

```bash
cd client
npm ci
npx ng serve
```

Open http://localhost:4200: the dev server proxies `/api` and `/uploads` to the API. `npx ng serve -c mock` runs the frontend against an in-memory mock without the backend.

Tests: `dotnet test` in `server/`, `npx ng test` in `client/`.

## Deployment

The demo runs on MonsterASP.NET (Windows/IIS, SQL Server). `deploy/deploy.sh` builds the Angular app into the API's `wwwroot`, publishes the API for `win-x86` and uploads it over SFTP using `app_offline.htm` to stop the running app. The production connection string lives only on the server in `appsettings.Production.json`.
