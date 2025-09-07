# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build Commands

### Solution Build
```bash
# Build entire solution (.NET 8)
dotnet build CryptoBot.sln -c Debug
dotnet build CryptoBot.sln -c Release

# Restore NuGet packages
dotnet restore CryptoBot.sln

# Legacy commands (if needed)
msbuild CryptoBot.sln /p:Configuration=Debug
nuget restore CryptoBot.sln
```

### UI Build (Angular)
```bash
cd CryptoBot.UI
npm install       # Install dependencies
npm run build     # Production build
npm run start     # Development server (port 3003)
npm run lint      # Run TypeScript linter
npm run lint-fix  # Fix linting issues
```

### Testing
```bash
# Run all tests (.NET 8)
dotnet test CryptoBot.Tests\CryptoBot.Tests.csproj

# Run tests with detailed output
dotnet test CryptoBot.Tests\CryptoBot.Tests.csproj --logger:"console;verbosity=detailed"

# Legacy command (if needed)
vstest.console.exe CryptoBot.Tests\bin\Debug\CryptoBot.Tests.dll

# Test Status: 19/19 tests passing (100% success rate)
```

## Architecture Overview

### Core Components

**CryptoBot.Core** - Central business logic and orchestration
- `Bots/` - Trading bot implementations and strategies
- `Trading/` - Core trading logic and trade execution
- `Ordering/` - Order management and placement
- `Scheduling/` - Task scheduling and time-based operations
- `Integrations/` - External service integrations
- `Messaging/` - Inter-component communication

**CryptoBot.ExchangeEngine** - Exchange connectivity layer
- `API/` - Exchange-specific API implementations (Coinbase Pro, Bittrex, etc.)
- `ExchangeFactory.cs` - Factory pattern for creating exchange connections
- Handles rate limiting, authentication, and API communication
- **Note**: GDAX APIs updated to Coinbase Pro endpoints

**CryptoBot.Model** - Domain models and entities
- Shared data models used across all projects
- Trading entities (Orders, Trades, Symbols, etc.)

**CryptoBot.Database** - Data persistence layer
- Entity Framework Core for SQL Server (.NET 8)
- Database context and migrations
- **Note**: Includes parameterless constructor for test mocking

**CryptoBot.Api** - REST API layer
- ASP.NET Core 2.0 Web API
- Controllers for exposing trading functionality
- Swagger documentation
- Uses AutoMapper for DTO mapping

**CryptoBot.UI** - Web frontend
- Angular 4.3.1 application
- Material Design UI components
- Webpack build configuration
- Real-time trading dashboard

### Strategy Components

**CryptoBot.IndicatorEngine** - Technical indicators
- Implements various trading indicators (RSI, MACD, etc.)
- Indicator calculation and signal generation

**CryptoBot.SafetyEngine** - Risk management
- Safety checks and risk controls
- Position sizing and exposure limits

**CryptoBot.BackTester** - Strategy backtesting
- Historical data testing framework
- Performance metrics and analysis

## Project Structure

The solution uses a layered architecture with clear separation of concerns:
- Exchange connectivity is isolated in ExchangeEngine
- Business logic resides in Core
- All projects reference the shared Model project
- The API project references Core, ExchangeEngine, and Model
- Tests use MSTest with Moq for mocking

## Framework Versions
- **.NET 8** (all projects - migrated from .NET Framework)
- ASP.NET Core 8.0 (API project)
- Entity Framework Core 8.0 (upgraded from EF 6.2.0)
- Angular 4.3.1 (UI project - unchanged)
- MSTest with Moq for unit testing

## Recent Updates (2024)
- ✅ **Complete .NET 8 Migration**: All projects successfully migrated
- ✅ **Test Suite Fixed**: 19/19 tests now passing (100% success rate)
- ✅ **API Modernization**: GDAX APIs updated to Coinbase Pro endpoints
- ✅ **Mocking Strategy**: Refactored from concrete class mocking to interface-based mocking
- ✅ **EF Core Upgrade**: Modern Include syntax and in-memory database support
- ✅ **Zero Build Warnings**: Complete warning cleanup across all projects