# =========================
# 1. Build Stage
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Install Node
RUN apt-get update && apt-get install -y nodejs npm

WORKDIR /src

COPY . .

RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# =========================
# 2. Runtime Stage
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "WatchHive.dll"]