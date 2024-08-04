#!/usr/bin/env fish

echo "Starting up! 🐠"

echo "Database..."
docker compose up db -d

sleep 5

echo "Migrating"
flyway migrate

echo "Kafka..."
docker compose up kafka kafka-connect kafdrop -d

sleep 15

echo "API & Load Balance"
docker compose up --build api nginx --scale api=2 -d

echo "Registering Connector "
curl --location 'http://localhost:8083/connectors/' \
--header 'Content-Type: application/json' \
--data '{
    "name": "core-ledger-connector",
    "config": {
        "connector.class": "io.debezium.connector.postgresql.PostgresConnector",
        "plugin.name": "pgoutput",
        "database.hostname": "db",
        "database.port": "5432",
        "database.user": "admin",
        "database.password": "password",
        "database.dbname": "service",
        "database.server.name": "db",
        "table.include.list": "public.ledger_accounts"
    }
}' --silent | jq
