#!/bin/bash

# Quick health check
echo "Checking services status..."

# Check SQL Server
if /opt/mssql-tools/bin/sqlcmd -S sqlserver -U sa -P "CryptoBot@2024!" -Q "SELECT 1" > /dev/null 2>&1; then
    echo "✓ SQL Server is running"
else
    echo "✗ SQL Server is not responding"
fi

# Check Redis
if redis-cli -h redis ping > /dev/null 2>&1; then
    echo "✓ Redis is running"
else
    echo "✗ Redis is not responding"
fi

# Set up git safe directory (for workspace)
git config --global --add safe.directory /workspace

echo ""
echo "CryptoBot DevContainer is ready!"
echo "Run 'code /workspace' to open the workspace in VS Code"