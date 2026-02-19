# POS Solution (Clean/Hexagonal)

## Projects
- Pos.Domain
- Pos.Application
- Pos.Infrastructure
- Pos.WebApi

## Conventions
- IDs: UUID (Guid)
- Base entities: Id, TenantId, CreatedAt, UpdatedAt, DeletedAt (optional)
- API base path: /api/v1

## Build
```bash
dotnet build backend/Pos.sln -m:1
```

## Test
Integration tests expect a running Postgres instance. It uses:
- `TEST_DB_CONNECTION` if set
- or defaults to `Host=localhost;Port=5432;Database=posdb;Username=pos;Password=pospass`

```bash
dotnet test backend/tests/Pos.Tests/Pos.Tests.csproj -m:1
```

## Migrations
Apply migrations:
```bash
dotnet ef database update -p backend/src/Pos.Infrastructure -s backend/src/Pos.WebApi
```

## Database Init SQL (DDL + optional seed)
Generate scripts from current model:
```bash
powershell -ExecutionPolicy Bypass -File backend/scripts/generate-init-and-seed.ps1
```

Generated files:
- `backend/scripts/sql/init.sql` -> DDL only (no seed)
- `backend/scripts/sql/seed.sql` -> optional sample data
- `backend/scripts/sql/reset.sql` -> drops and recreates `public` schema

Reset + initialize manually:
```bash
docker exec -i pos-postgres psql -U pos -d posdb < backend/scripts/sql/reset.sql
docker exec -i pos-postgres psql -U pos -d posdb < backend/scripts/sql/init.sql
docker exec -i pos-postgres psql -U pos -d posdb < backend/scripts/sql/seed.sql
```

Notes:
- `docker-compose` mounts `init.sql` into `/docker-entrypoint-initdb.d`, so it runs automatically only on a brand-new Postgres volume.
- `seed.sql` is not auto-applied by Docker; run it only when needed.

## Run WebApi (local)
```bash
dotnet run --project backend/src/Pos.WebApi/Pos.WebApi.csproj
```

Sample endpoints:
- `GET /api/v1/health` -> returns status `ok`
- `POST /api/v1/auth/login` -> returns `{ token, expiresAt }`

## Frontend (Vue + Vuetify)
From the repo root:
```bash
cd frontend/pos-ui
npm install
```

Run dev server:
```bash
npm run dev
```

API base URL (optional):
```bash
set VITE_API_BASE_URL=http://localhost:8080
```

Login at `http://localhost:5173/login`.

## Docker Compose
Start:
```bash
docker compose up -d
```
Logs:
```bash
docker compose logs -f pos-api
```

Optional pgAdmin:
```bash
docker compose --profile tools up -d
```

Health:
```bash
curl http://localhost:8080/api/v1/health
```
