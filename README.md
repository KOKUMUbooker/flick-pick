# Movie Management API

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
git clone <your-repo-url>
cd Movie-manager
```

2. **Start PostgreSQL**
```bash
docker-compose up -d
```
Database: `moviemanager` | User: `admin` | Password: `secret` | Port: `5480`

3. **Install EF Core tools**
```bash
dotnet tool restore
```

4. **Apply migrations**
```bash
dotnet ef database update
```

5. **Run the API**
```bash
dotnet run
```

API runs on `https://localhost:7189`

## 📖 API Documentation

Once running, explore the API:
- **Swagger UI**: `https://localhost:7189/swagger`
- **Interactive docs** with full endpoint details

### Endpoints
- `GET /api/movies` - List all movies
- `GET /api/movies/{id}` - Get specific movie
- `POST /api/movies` - Create new movie
- `PUT /api/movies/{id}` - Update movie
- `DELETE /api/movies/{id}` - Delete movie

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
MovieManager/
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
"DefaultConnection": "Host=localhost;Port=5480;Database=moviemanager;Username=admin;Password=secret"
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
