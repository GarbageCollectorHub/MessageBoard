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

Install `dotnet-ef` if it is not installed:

```bash
dotnet tool install --global dotnet-ef --version 10.0.14
```

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

## Comment Markup

Allowed tags: `a`, `code`, `i`, `strong`. Links support `href` (HTTP/HTTPS) and `title`; other tags have no attributes.
Tags must be correctly closed and nested. URLs must not contain a login or password.

Write special characters as XML entities: `&amp;` for `&` and `&lt;` for `<` in text and attribute values.

## Documentation

See [Technical Decisions](docs/DECISIONS.md) for key implementation decisions.
