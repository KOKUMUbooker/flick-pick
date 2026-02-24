# =========================
# 1️⃣ Build Svelte UI
# =========================
FROM node:lts-alpine AS ui-build

WORKDIR /app/UI

COPY UI/package*.json ./
RUN npm install

COPY UI/ .
RUN npm run build


# =========================
# 2️⃣ Build ASP.NET Core
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy only csproj first (better layer caching)
COPY WatchHive.csproj .
RUN dotnet restore WatchHive.csproj

# Copy everything else
COPY . .

# Replace wwwroot with built UI
RUN rm -rf wwwroot/*
COPY --from=ui-build /app/UI/build ./wwwroot

# Publish
RUN dotnet publish WatchHive.csproj -c Build --self-contained false -o /app/publish


# =========================
# 3️⃣ Runtime Image
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

RUN export PORT=5000

ENTRYPOINT ["dotnet", "WatchHive.dll"]