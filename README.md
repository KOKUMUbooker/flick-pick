# FlickPick API

A .NET 8 Web API for managing movies with CRUD operations, PostgreSQL, and Minimal APIs.

## Features

- Full CRUD operations for movie management
- PostgreSQL database with Docker
- Entity Framework Core with code-first migrations
- Minimal API endpoints
- Automatic data seeding
- OpenAPI documentation

## Quick Start

### Prerequisites

- .NET 8 SDK
- Docker
- Git

### Installation

1. **Clone and setup**

```bash
git clone <repo-url>
cd Movie-manager
```

2. **Start PostgreSQL**

```bash
docker compose up -d
```

Database: `flickpick` | User: `admin` | Password: `secret` | Port: `5480`

3. **Install EF Core tools**

```bash
dotnet tool restore
```

4. **Create & Apply migrations**

```bash
rm -r Migrations/
```

If database contained some data first (for local development)

```bash
docker exec -it postgres psql -U admin -d flickpick -c "DROP SCHEMA app CASCADE;
```

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

5. **Run the API**

```bash
dotnet run
```

API runs on `https://localhost:5167`

## API Documentation

Once running, explore the API:

- **Swagger UI**: `https://localhost:5167/swagger`
- **Interactive docs** with full endpoint details

### Endpoints


## Development

### Useful Commands

```bash
# Run with auto-reload
dotnet watch run

# Create new migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Stop PostgreSQL
docker-compose down
```

### Project Structure

```
FlickPickApp/
├── Models/          # Domain entities & DB context
├── DTOs/           # Data transfer objects
├── Services/       # Business logic
├── Endpoints/      # API endpoints
├── Migrations/     # Database migrations
└── Utils/          # Database seeding
```

## Configuration

Connection string in `appsettings.json`:

```json
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5480;Database=flickpick;Username=admin;Password=secret;"
  }
```

## Troubleshooting

**Can't connect to database?**

```bash
# Check if PostgreSQL is running
docker ps
# Should show 'postgres' container

# View logs
docker-compose logs postgres
```

**Migrations not working?**

```bash
# Reset everything
docker-compose down -v
docker-compose up -d
rm -rf Migrations/
dotnet ef migrations add InitialCreate
dotnet ef database update
```

**API won't start?**

```bash
# Clean and rebuild
dotnet clean
dotnet build
dotnet run
```
