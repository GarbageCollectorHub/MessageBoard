# MessageBoard

MessageBoard is an ASP.NET Core Web API for comments and replies. The project is under development.

## Features

- Create comments and replies to existing comments.
- Validate input, URLs, and supported markup.
- Return structured API errors and log unexpected failures.

## Tech Stack

- C# / .NET 10, ASP.NET Core Web API
- Entity Framework Core, SQL Server / Azure SQL
- FluentValidation
- Serilog
- OpenAPI / Swagger UI
- xUnit

## Project Structure

```text
backend/
├── MessageBoard.Api             # Controllers, API configuration
├── MessageBoard.Application     # Business logic, validation
├── MessageBoard.Domain          # Entities, constants
├── MessageBoard.Infrastructure  # EF Core, repositories, migrations
└── MessageBoard.Tests           # Unit tests
```

## Getting Started

You'll need the .NET 10 SDK, SQL Server or Azure SQL, and the `dotnet-ef` tool.

From the repository root, open the `backend` directory:

```bash
cd backend
```

Set your database connection string using .NET User Secrets:

```bash
dotnet user-secrets set "ConnectionStrings:MessageBoard" "YOUR_CONNECTION_STRING" --project MessageBoard.Api
```

Apply the database migrations and run the API:

```bash
dotnet ef database update --project MessageBoard.Infrastructure --startup-project MessageBoard.Api
dotnet run --project MessageBoard.Api --launch-profile https
```

Swagger UI (Development): https://localhost:7238/swagger

Run the tests from `backend`:

```bash
dotnet test MessageBoard.slnx
```

## Documentation

See [Technical Decisions](docs/DECISIONS.md) for key implementation decisions.
