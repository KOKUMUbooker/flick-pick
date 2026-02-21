#! /bin/bash

set -e

echo "🛢️🛢️  Deleting Database 🤤🤤"
docker exec -it postgres psql -U admin -d watchhive -c "DROP SCHEMA app CASCADE;"

echo "🛑🛑 Stopping the postgres docker container 🛑🛑"
docker compose -f docker-compose-db.yaml down -v

echo "🚀🚀 Stating up the postgess docker container 🚀🚀"
docker compose -f docker-compose-db.yaml up -d

echo "🌞🌞 Applying migrations on the db 🌞🌞"
dotnet ef database update
