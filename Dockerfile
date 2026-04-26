# =========================
# 1️⃣ Build Svelte UI
# =========================
FROM node:20-alpine AS ui-build

WORKDIR /app/UI

COPY UI/package*.json ./
RUN npm ci

COPY UI/ .
RUN npm run build


# =========================
# 2️⃣ Build ASP.NET Core (.NET 10)
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build

WORKDIR /src

# Copy project files first (cache restore)
COPY WatchHive.csproj ./
RUN dotnet restore

# Copy everything else
COPY . .

# Replace wwwroot with built UI
RUN rm -rf wwwroot/*
COPY --from=ui-build /app/UI/build ./wwwroot

# Publish
RUN dotnet publish -c Release -o /app/publish \
    -p:UseAppHost=false \
    -p:PublishTrimmed=false


# =========================
# 3️⃣ Runtime (.NET 10)
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS runtime

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 5000

ENTRYPOINT ["dotnet", "WatchHive.dll"]