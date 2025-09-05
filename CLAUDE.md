# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build Commands

### Solution Build
```bash
# Build entire solution
msbuild CryptoBot.sln /p:Configuration=Debug
msbuild CryptoBot.sln /p:Configuration=Release

# Restore NuGet packages
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
# Run MSTest tests
vstest.console.exe CryptoBot.Tests\bin\Debug\CryptoBot.Tests.dll
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
- `API/` - Exchange-specific API implementations (GDAX, etc.)
- `ExchangeFactory.cs` - Factory pattern for creating exchange connections
- Handles rate limiting, authentication, and API communication

**CryptoBot.Model** - Domain models and entities
- Shared data models used across all projects
- Trading entities (Orders, Trades, Symbols, etc.)

**CryptoBot.Database** - Data persistence layer
- Entity Framework 6.2.0 for SQL Server
- Database context and migrations

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
- .NET Framework 4.6.2 (most projects)
- .NET Framework 4.5.2 (test project)
- ASP.NET Core 2.0 (API project)
- Angular 4.3.1 (UI project)
- Entity Framework 6.2.0