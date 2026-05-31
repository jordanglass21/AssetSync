#!/bin/bash

echo "--- Clearing ports ---"
lsof -ti:8080 | xargs kill -9 2>/dev/null
lsof -ti:5289 | xargs kill -9 2>/dev/null
lsof -ti:5173 | xargs kill -9 2>/dev/null

echo "--- Starting AssetSync ---"

(cd AssetSync.Legacy && ./mvnw spring-boot:run) &
LEGACY_PID=$!

(cd AssetSync.Api && dotnet run) &
API_PID=$!

(cd AssetSync.Client && npm run dev) &
CLIENT_PID=$!

echo "Legacy PID: $LEGACY_PID"
echo "API PID: $API_PID"
echo "Client PID: $CLIENT_PID"

echo "All services started. Press Ctrl+C to stop."

wait