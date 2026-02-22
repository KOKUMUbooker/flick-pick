# 🎬 WatchHive - Stop Debating, Start Watching

WatchHive is a decision-making platform that helps friends, couples, and families choose movies together—without the endless group chat debates. We solve the coordination problem so you can focus on the fun part: actually watching.

---

## The Problem

### The Movie Night Planning Nightmare:

- **30+ minutes** lost debating in group chats
- **No fair system** - someone always compromises
- **Scheduling headaches** across time zones and busy lives
- **Streaming confusion** - "Who has Netflix? Does anyone have Hulu?"
- **Lost conversations** - Great discussions disappear in chat history

**Result:** Decision fatigue, frustration, or movie night never happens.

---

## The Solution

WatchHive transforms chaotic planning into a **simple 4-step process:**

1. **Create** a group for your movie nights
2. **Suggest & Vote** on movies with fair voting (up/down + veto power)
3. **Schedule** a time that works for everyone
4. **Watch & Discuss** with post-movie ratings and conversations

**We don't stream movies—we help you decide which ones to watch.**

---

## Core Features

### Smart Voting System

- **Upvote/downvote** movies in real-time
- **One veto per person** to prevent absolute no-gos
- **Live results** so everyone sees what's winning
- **Streaming availability check** before voting

### Group Coordination

- **Persistent groups** for your friend circles
- **Anonymous voting** - friends can vote without accounts
- **Easy invites** with shareable links
- **Member management** with admin controls

### Decision & Scheduling

- **Calendar integration** to find mutual availability
- **Time zone support** for long-distance friends
- **Reminders & notifications** for upcoming movie nights
- **Watch tracking** - mark when you actually start watching

### Post-Movie Experience

- **Group ratings** (1-10 with averages)
- **Watch history** - see all movies your group has watched

## Tech Stack

### Frontend

- **Svelte** + **SvelteKit** - Reactive, fast UI
- **TypeScript** - Type-safe development
- **Tailwind CSS** + **shadcn-svelte** - Beautiful, consistent components
- **Lucide Icons** - Clean iconography

### Backend

- **ASP.NET Core** - Robust API framework
- **Entity Framework Core** - Database ORM
- **PostgreSQL** - Relational database with JSONB support
- **SignalR** - Real-time voting and chat

### APIs & Services

- **TMDB API** - Movie metadata and details
- **JWT Authentication** - Secure user sessions

---

## Quick Start

### Prerequisites

- .NET 8 SDK
- Docker
- Git

### Installation

1. **Clone and setup**

```bash
git clone <repo-url>
cd watch-hive
```

2. **Start PostgreSQL**

```bash
docker compose -f docker-compose-db.yaml up -d
```

1. **Install EF Core tools**

```bash
dotnet tool restore
```

4. **Create & Apply migrations**

```bash
rm -r Migrations/
```

If database contained some data first (for local development)

```bash
docker exec -it postgres psql -U admin -d WatchHive -c "DROP SCHEMA app CASCADE;
```

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

5. **Run the API**

```bash
dotnet run
```

OR via docker

1. Build image

```bash
docker build -t watchhive:1.0 .
```

2. Start image

```bash
docker run --env-file .env watchhive
```

OR via docker compose

```bash
docker compose -f docker-compose-server.yaml up -d
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
WatchHive/
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
    "DefaultConnection": "Host=localhost;Port=5480;Database=watchhive;Username=admin;Password=secret;"
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
