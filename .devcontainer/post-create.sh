#!/bin/bash

echo "Setting up CryptoBot development environment..."

# Restore NuGet packages for .NET Framework projects
echo "Restoring NuGet packages..."
cd /workspace
nuget restore CryptoBot.sln

# Install npm packages for UI
echo "Installing Angular UI dependencies..."
cd /workspace/CryptoBot.UI
npm install

# Install .NET Core dependencies for API
echo "Restoring .NET Core API dependencies..."
cd /workspace/CryptoBot.Api
dotnet restore

# Wait for SQL Server to be ready
echo "Waiting for SQL Server to be ready..."
until /opt/mssql-tools/bin/sqlcmd -S sqlserver -U sa -P "CryptoBot@2024!" -Q "SELECT 1" > /dev/null 2>&1; do
    echo "SQL Server is not ready yet. Waiting..."
    sleep 5
done
echo "SQL Server is ready!"

# Create database if it doesn't exist
echo "Creating CryptoBot database if needed..."
/opt/mssql-tools/bin/sqlcmd -S sqlserver -U sa -P "CryptoBot@2024!" -Q "IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'CryptoBot') CREATE DATABASE CryptoBot"

# Run Entity Framework migrations if available
if [ -f "/workspace/CryptoBot.Database/Migrations" ]; then
    echo "Running Entity Framework migrations..."
    cd /workspace/CryptoBot.Database
    # This would need adjustment based on your migration approach
fi

echo "Development environment setup complete!"
echo ""
echo "Available commands:"
echo "  - Build solution: msbuild CryptoBot.sln"
echo "  - Run API: cd CryptoBot.Api && dotnet run"
echo "  - Run UI: cd CryptoBot.UI && npm start"
echo "  - Run tests: vstest.console.exe CryptoBot.Tests/bin/Debug/CryptoBot.Tests.dll"
echo ""
echo "Services:"
echo "  - SQL Server: localhost:1433 (sa/CryptoBot@2024!)"
echo "  - Redis: localhost:6379"
echo "  - API: http://localhost:5000"
echo "  - UI: http://localhost:3003"