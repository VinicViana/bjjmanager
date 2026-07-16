# BJJManager

A web application for Brazilian Jiu-Jitsu practitioners to track training sessions and build a
personal library of techniques.

Full functional/technical specs and execution plans that drove this implementation live in
[`specs/`](specs/00-master-spec.md).

## Features

- JWT-based authentication (register/login)
- Training session log with CRUD, filtering by exact date/month/year, and a self-evaluation score
  (1–5) per session
- Technique library with CRUD, ordered step lists, and a free-text/dropdown position field
- Photo/video attachments on both trainings and techniques, uploaded client-side straight to
  Firebase Storage — the API only ever stores the resulting download URL
- Dashboard with totals plus a daily-average line chart of training self-evaluation scores over
  the last 30 days
- Floating "AI Coach" chat widget, backed by the OpenAI API — answers jiu-jitsu questions in
  whatever language they're asked in; nothing about the conversation is persisted anywhere
- Responsive Angular Material UI with a collapsible sidebar

## Stack

- **Backend**: .NET 8, ASP.NET Core Web API, MediatR (CQRS), EF Core, FluentValidation, JWT auth,
  Clean Architecture (Domain / Application / Infrastructure / WebApi)
- **Frontend**: Angular 18 (standalone components, signals, Angular Material), Firebase Storage
  SDK for media uploads
- **Database**: SQL Server, EF Core Code First migrations

## Repository layout

```
/backend      .NET solution (src/ + tests/)
/frontend     Angular workspace (bjj-manager-app/)
/specs        Master spec + backend/frontend/database execution plans
```

## Prerequisites

- .NET 8 SDK (or newer, with the `net8.0` runtime installed)
- Node.js 18+ and npm
- A local SQL Server instance (developed against SQL Server Express)
- A Firebase project with Storage enabled, if you want media uploads to work (see
  `frontend/bjj-manager-app/src/environments/environment.ts` for where the config goes)
- An OpenAI API key, if you want the AI Coach chat to work (see step 2)

## 1. Database setup

Create an empty database matching `ConnectionStrings:DefaultConnection` in
`backend/src/BJJManager.WebApi/appsettings.Development.json` (defaults to `BJJManager` on
`localhost\SQLEXPRESS02` — adjust to your own instance if it differs):

```
sqlcmd -S "localhost\SQLEXPRESS02" -E -Q "CREATE DATABASE BJJManager"
```

That's it — schema migrations and demo data seeding happen automatically the first time the API
runs (see step 2).

## 2. Run the backend

```
cd backend
cp src/BJJManager.WebApi/appsettings.Local.json.example src/BJJManager.WebApi/appsettings.Local.json
dotnet test        # 45 tests: domain, application (unit) + WebApi (integration, SQLite in-memory)
dotnet run --project src/BJJManager.WebApi
```

`appsettings.Local.json` is gitignored — put your own `OpenAI:ApiKey` in the copy for the AI Coach
chat to work; the rest of the app works without it.

API listens on `http://localhost:5144` by default (see `Properties/launchSettings.json`), with
Swagger UI at `http://localhost:5144/swagger`.

## 3. Run the frontend

```
cd frontend/bjj-manager-app
cp src/environments/environment.example.ts src/environments/environment.ts
npm install
npm test -- --watch=false --browsers=ChromeHeadless   # 32 tests
npm start
```

Serves on `http://localhost:4200`, calling the API at `http://localhost:5144/api`.

`environment.ts` is gitignored — fill in your own Firebase project config (Storage enabled) in the
copy for photo/video uploads to work; the rest of the app works without it.

## Demo credentials

Seeded automatically on first backend run:

- **Name:** `demo`
- **Password:** `Demo@123`

Comes with 3 sample training sessions and 2 sample techniques (with steps) already populated.

## Notes

- No code comments throughout the codebase, per project convention.
