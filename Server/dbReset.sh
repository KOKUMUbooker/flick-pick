#! /bin/bash

set -e

echo "🛢️🛢️  Deleting Database 🤤🤤"
docker exec -it postgres psql -U admin -d flickpick -c "DROP SCHEMA app CASCADE;"

echo "🛑🛑 Stopping the postgres docker container 🛑🛑"
docker compose down -v

echo "🚀🚀 Stating up the postgess docker container 🚀🚀"
docker compose up -d

echo "🌞🌞 Applying migrations on the db 🌞🌞"
dotnet ef database update
