# .NET 8 Migration TODO List

## 📋 Migration Tasks

### ✅ Completed
- [x] Analyze all project files to understand current .NET versions and dependencies
- [x] Research compatibility issues between .NET Framework and .NET 8
- [x] Create comprehensive migration guide
- [x] Create backup reminder and pre-migration checklist

### 🚀 In Progress
None currently active

### 📝 Pending Tasks

#### Phase 1: Preparation
- [ ] **Create full backup of solution**
  - [ ] Create git branch for migration
  - [ ] Document current build/test status
  - [ ] Export database schema and sample data

- [ ] **Install prerequisites**
  - [ ] Install .NET 8 SDK
  - [ ] Update Visual Studio to 2022 (v17.8+)
  - [ ] Install .NET Upgrade Assistant tool

#### Phase 2: Base Project Migration
- [ ] **Migrate CryptoBot.Model project to .NET 8**
  - [ ] Convert to SDK-style project
  - [ ] Update TargetFramework to net8.0
  - [ ] Remove packages.config
  - [ ] Test compilation

#### Phase 3: Data Layer Migration
- [ ] **Migrate CryptoBot.Database project to .NET 8 with EF Core**
  - [ ] Convert to SDK-style project
  - [ ] Replace Entity Framework 6 with EF Core 8
  - [ ] Recreate database migrations
  - [ ] Update DbContext for EF Core
  - [ ] Update connection string handling
  - [ ] Test database connectivity

#### Phase 4: Service Layer Migration
- [ ] **Migrate CryptoBot.ExchangeEngine project to .NET 8**
  - [ ] Convert to SDK-style project
  - [ ] Update package references
  - [ ] Replace WebSocketSharp with System.Net.WebSockets
  - [ ] Fix breaking API changes

- [ ] **Migrate CryptoBot.IndicatorEngine project to .NET 8**
  - [ ] Convert to SDK-style project
  - [ ] Update TA-Lib references
  - [ ] Update Trady libraries
  - [ ] Test indicator calculations

- [ ] **Migrate CryptoBot.SafetyEngine project to .NET 8**
  - [ ] Convert to SDK-style project
  - [ ] Update package references
  - [ ] Test safety checks

#### Phase 5: Core Migration
- [ ] **Migrate CryptoBot.Core project to .NET 8**
  - [ ] Convert to SDK-style project
  - [ ] Update all service references
  - [ ] Migrate configuration system
  - [ ] Update Telegram.Bot to v19+
  - [ ] Fix dependency injection

#### Phase 6: Application Migration
- [ ] **Migrate CryptoBot.BackTester project to .NET 8**
  - [ ] Convert to SDK-style project
  - [ ] Update references
  - [ ] Test backtesting functionality

- [ ] **Migrate CryptoBot.Console project to .NET 8**
  - [ ] Convert to SDK-style project
  - [ ] Remove ClickOnce configuration
  - [ ] Migrate from app.config to appsettings.json
  - [ ] Update all package references
  - [ ] Consider top-level program style

- [ ] **Migrate CryptoBot.Tests project to .NET 8**
  - [ ] Convert to SDK-style project
  - [ ] Update MSTest or migrate to xUnit
  - [ ] Update Moq framework
  - [ ] Fix all test compilation issues
  - [ ] Run all tests

- [ ] **Migrate CryptoBot.Api project to .NET 8**
  - [ ] Update from ASP.NET Core 2.0 to 8.0
  - [ ] Update Swashbuckle to 6.5+
  - [ ] Update middleware configuration
  - [ ] Test all API endpoints

#### Phase 7: Integration & Testing
- [ ] **Full solution testing**
  - [ ] Run all unit tests
  - [ ] Run integration tests
  - [ ] Test exchange connections
  - [ ] Test trading functionality
  - [ ] Performance benchmarking

#### Phase 8: Documentation & Cleanup
- [ ] **Create migration documentation**
  - [ ] Document breaking changes encountered
  - [ ] Update README.md
  - [ ] Update CLAUDE.md with new build commands
  - [ ] Create deployment guide

- [ ] **Clean up**
  - [ ] Remove old .NET Framework files
  - [ ] Remove packages folders
  - [ ] Update .gitignore
  - [ ] Update CI/CD pipelines

## 📊 Progress Tracker

| Component | Status | Progress |
|-----------|--------|----------|
| Analysis & Planning | ✅ Complete | 100% |
| CryptoBot.Model | ⏳ Pending | 0% |
| CryptoBot.Database | ⏳ Pending | 0% |
| CryptoBot.ExchangeEngine | ⏳ Pending | 0% |
| CryptoBot.IndicatorEngine | ⏳ Pending | 0% |
| CryptoBot.SafetyEngine | ⏳ Pending | 0% |
| CryptoBot.Core | ⏳ Pending | 0% |
| CryptoBot.BackTester | ⏳ Pending | 0% |
| CryptoBot.Console | ⏳ Pending | 0% |
| CryptoBot.Tests | ⏳ Pending | 0% |
| CryptoBot.Api | ⏳ Pending | 0% |
| Testing & Validation | ⏳ Pending | 0% |
| Documentation | ⏳ Pending | 0% |

**Overall Progress: 8% (1/13 major tasks)**

## 🔥 High Priority Issues
- Entity Framework 6 to EF Core 8 migration (most complex)
- WebSocketSharp replacement required
- Configuration system overhaul (app.config to appsettings.json)

## 📝 Notes
- Start with CryptoBot.Model as it has no dependencies
- EF Core migration will require the most time and testing
- Consider running old and new versions in parallel during transition
- Keep detailed notes of all breaking changes for documentation

## 🎯 Next Steps
1. Create solution backup
2. Install .NET 8 SDK and Visual Studio 2022
3. Start with CryptoBot.Model migration as proof of concept

---
*Last Updated: September 2024*
*Use this checklist to track migration progress*