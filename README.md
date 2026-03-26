# V&Qs (Vote and Quiz Platform)

A full-stack web application for creating, browsing, and participating in **votes** and **quizzes**.

- **Backend:** ASP.NET Core 8 Web API + ASP.NET Core Identity + Entity Framework Core
- **Frontend:** React + Vite
- **Database:** SQL Server (configured via connection string)

---

## Project Structure

```text
V-Qs/
├─ VoteAndQuizWebApi.sln
├─ VoteAndQuizWebApi/
│  ├─ Controllers/                # API controllers (HTTP layer)
│  ├─ Services/                   # Business logic layer
│  │  ├─ Interfaces/              # Service contracts
│  │  ├─ QuizzesService.cs
│  │  ├─ VotesService.cs
│  │  └─ ServiceResult.cs
│  ├─ Repository/                 # Repository + UnitOfWork data access
│  ├─ Data/                       # DbContext and DB seeding
│  ├─ Models/                     # Domain entities
│  ├─ Dto/                        # API DTOs
│  ├─ Migrations/                 # EF Core migrations
│  ├─ frontendReact/              # React app (Vite)
│  ├─ Program.cs                  # App configuration and middleware pipeline
│  ├─ appsettings.json
│  └─ Properties/launchSettings.json
└─ README.md
```

---

## Features

- Authentication using ASP.NET Core Identity endpoint mapping (`/register`, `/login`, `/logout`)
- Protected vote and quiz APIs (authorized users)
- Create, list, view details, vote, finish, and soft-delete for votes/quizzes
- Quiz correct answer retrieval after quiz is finished
- Initial data seeding on startup (`admin` user + sample vote + sample quiz)
- Swagger UI for API testing in development

---

## Architecture (Current)

The backend uses layered architecture:

1. **Controllers**: request/response handling only
2. **Services**: business logic (`IQuizzesService`, `IVotesService`)
3. **Repository + UnitOfWork**: persistence abstraction over EF Core
4. **Data/DbContext**: EF Core mappings and database interactions

Service operations return `ServiceResult` / `ServiceResult<T>` for unified error handling.

---

## Prerequisites

- .NET SDK 8.x
- SQL Server (local instance, currently configured for `localhost\\SQLEXPRESS`)
- Node.js 18+ and npm
- Trusted ASP.NET local HTTPS certificate

(Optional)
- EF CLI tools (`dotnet tool install --global dotnet-ef`)

---

## Configuration

### Backend connection string

In `VoteAndQuizWebApi/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=VoteAndQuiz;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;"
}
```

Update this value if your SQL Server instance is different.

### Launch profiles and ports

`VoteAndQuizWebApi/Properties/launchSettings.json`:

- HTTP: `http://localhost:5073`
- HTTPS: `https://localhost:7055` (also binds HTTP `5073`)

### Frontend dev server

`VoteAndQuizWebApi/frontendReact/vite.config.js`:

- Vite dev server: `http://localhost:7056`

---

## Getting Started

## 1) Restore backend dependencies

From `VoteAndQuizWebApi`:

```bash
dotnet restore
```

## 2) Ensure HTTPS dev certificate is trusted

```bash
dotnet dev-certs https --trust
```

## 3) Run backend

```bash
dotnet run --launch-profile https
```

The app seeds data on startup via `SeedDatabase()` in `Program.cs`.

## 4) Run frontend

From `VoteAndQuizWebApi/frontendReact`:

```bash
npm install
npm run dev
```

---

## Swagger / API Testing

When backend runs in Development:

- `https://localhost:7055/swagger`
- or `http://localhost:5073/swagger`

Use Swagger to test endpoints. For protected endpoints, authenticate first via Identity login endpoint and keep cookies in the same browser session.

---

## Seeded User

On first startup, `DbInitializer` creates:

- **Email:** `admin@iskren.com`
- **Password:** `Qqq123*`

It also seeds one sample quiz and one sample vote (if DB is empty).

---

## Main API Routes

### Identity (mapped by `MapIdentityApi<User>()`)

- `POST /register`
- `POST /login`
- `POST /logout` (custom mapped endpoint in `Program.cs`, authorized)
- `GET /pingauth` (custom mapped endpoint in `Program.cs`, authorized)

### Votes (`api/Votes`)

- `GET /api/Votes`
- `GET /api/Votes/Details/{id}`
- `POST /api/Votes/Create`
- `GET /api/Votes/Result/{id}`
- `DELETE /api/Votes/Delete/{id}`
- `POST /api/Votes/Finish/{id}`
- `POST /api/Votes/Vote/{id}/{voteOptionId}`

### Quizzes (`api/Quizzes`)

- `GET /api/Quizzes`
- `GET /api/Quizzes/Details/{id}`
- `POST /api/Quizzes/Create`
- `DELETE /api/Quizzes/Delete/{quizId}`
- `PUT /api/Quizzes/{quizId}/vote/{answerId}`
- `POST /api/Quizzes/Finish/{quizId}`
- `GET /api/Quizzes/CorrectOption/{quizId}`

---

## Notes for Local Development

- If login fails with `Failed to fetch`, verify backend is running on the same port expected by frontend requests.
- Current frontend code uses hardcoded backend URLs (`https://localhost:7055`) in several components.
- If `dotnet build` fails due locked files, stop any running `VoteAndQuizWebApi.exe` process first.

---

## Useful Commands

### Backend

```bash
dotnet build
dotnet run --launch-profile https
```

### Frontend

```bash
npm install
npm run dev
npm run build
```

### Migrations (if needed)

From `VoteAndQuizWebApi`:

```bash
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

---

## .gitignore Recommendation

Do not commit build/runtime artifacts:

- `VoteAndQuizWebApi/bin/`
- `VoteAndQuizWebApi/obj/`
- `VoteAndQuizWebApi/frontendReact/node_modules/`
- temporary local folders such as `temp-build-output/`

---

## Tech Stack Summary

- ASP.NET Core 8 (Web API + Identity endpoints)
- Entity Framework Core + SQL Server
- React 18 + Vite