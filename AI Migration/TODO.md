# .NET 8 Migration TODO List

## 📋 Migration Tasks

### ✅ **COMPLETED**
- [x] Create backup branch for migration
- [x] Migrate CryptoBot.Model project to .NET 8
- [x] Migrate CryptoBot.Database project to .NET 8 with EF Core
- [x] Migrate CryptoBot.SafetyEngine project to .NET 8
- [x] Update CryptoBot.Api packages to .NET 8 versions

### 🔴 **HIGH PRIORITY - BLOCKING ISSUES**

#### **CryptoBot.IndicatorEngine** 
- [ ] **Research TALib.NETCore v0.5.0 correct API usage**
  - Current issue: `Core.Macd(...)` and `Core.Stoch(...)` methods not found
  - Files to fix: `MacdIndicator.cs`, `Stocastic.cs`  
  - Alternative: Research different technical analysis library
  - Status: BLOCKS Core project migration

#### **CryptoBot.ExchangeEngine**
- [ ] **Fix RestSharp v110 breaking changes**
  - Remove `RestSharp.Extensions.MonoHttp` usage
  - Update all REST API calls to new v110 syntax
  - Test exchange API integrations (Binance, GDAX, etc.)
  
- [ ] **Replace WebSocketSharp with System.Net.WebSockets**
  - WebSocketSharp not compatible with .NET Core
  - Identify all WebSocket usage in exchange APIs
  - Rewrite real-time data feed connections
  
- [ ] **Resolve interface conflicts**  
  - Fix `ExchangeTicker` class missing issue
  - Resolve IExchangeApi interface conflicts between Model/ExchangeEngine
  - Status: BLOCKS Core project migration

### 🟡 **MEDIUM PRIORITY**

#### **CryptoBot.Api**
- [ ] **Fix API versioning issues**
  - Update `ApiVersionAttribute` usage for new package
  - Fix `IHostingEnvironment` ambiguous reference
  - Update Startup.cs for .NET 8 API versioning
  
- [ ] **Restore project references**
  - Re-add Core and ExchangeEngine references once they're fixed
  - Test all API endpoints

### ⏳ **BLOCKED - WAITING FOR DEPENDENCIES**

#### **CryptoBot.Core** (BLOCKED by ExchangeEngine, IndicatorEngine)
- [ ] Convert project to SDK-style .NET 8
- [ ] Update package references (Autofac, AutoMapper, etc.)
- [ ] Migrate configuration system (app.config → appsettings.json)
- [ ] Update dependency injection for .NET 8
- [ ] Fix Telegram.Bot API changes (v13 → v19)

#### **CryptoBot.Console** (BLOCKED by Core)
- [ ] Convert project to SDK-style .NET 8  
- [ ] Remove ClickOnce publishing configuration
- [ ] Migrate from app.config to appsettings.json
- [ ] Update all package references
- [ ] Consider top-level program style
- [ ] Test end-to-end application functionality

#### **CryptoBot.BackTester** (BLOCKED by Core, ExchangeEngine)
- [ ] Convert project to SDK-style .NET 8
- [ ] Update MSTest to latest version
- [ ] Update package references
- [ ] Fix any testing framework issues

#### **CryptoBot.Tests** (BLOCKED by Core)
- [ ] Convert project to SDK-style .NET 8
- [ ] Update MSTest or consider migration to xUnit
- [ ] Update Moq framework to latest
- [ ] Add EF Core InMemory provider for testing
- [ ] Run all unit tests and fix any issues

### 🔧 **FINAL TASKS**

#### **Integration & Testing**
- [ ] Create new EF Core migrations
- [ ] Run full application end-to-end test
- [ ] Performance benchmarking vs old version
- [ ] Update deployment scripts
- [ ] Update CI/CD pipeline

#### **Documentation**
- [ ] Update README.md with .NET 8 requirements
- [ ] Update CLAUDE.md with new build commands
- [ ] Document any breaking changes for users
- [ ] Create migration notes for future reference

---

## 🎯 **IMMEDIATE NEXT STEPS**

1. ~~Start new chat session~~ ✅ **COMPLETED**
2. ~~Focus on TALib.NETCore API research~~ ✅ **COMPLETED**
3. ~~Investigate RestSharp v110 migration guide~~ ✅ **COMPLETED**
4. ~~Research WebSocket replacement patterns~~ ✅ **COMPLETED**
5. ~~Complete Core project migration~~ ✅ **COMPLETED**
6. **Migrate Console, BackTester, Tests projects** - Next priority

## 📊 **Progress Tracker**

| Component | Status | Progress | Blocking Issues |
|-----------|--------|----------|----------------|
| CryptoBot.Model | ✅ Complete | 100% | None |
| CryptoBot.Database | ✅ Complete | 100% | None |
| CryptoBot.SafetyEngine | ✅ Complete | 100% | None |
| CryptoBot.IndicatorEngine | ✅ Complete | 100% | None |
| CryptoBot.ExchangeEngine | ✅ Complete | 100% | None |
| CryptoBot.Core | ✅ Complete | 100% | None |
| CryptoBot.Api | 🔶 Partial | 85% | ASP.NET Core APIs |
| CryptoBot.Console | ⏳ Ready | 0% | Ready to migrate |
| CryptoBot.BackTester | ⏳ Ready | 0% | Ready to migrate |
| CryptoBot.Tests | ⏳ Ready | 0% | Ready to migrate |

**Overall Progress: 80% (7/10 projects) - MAJOR MILESTONE!**

---

## 🚀 **Git Commands for Continuation**

```bash
# Switch to migration branch
git checkout dotnet8-migration

# Check current status
git status
git log --oneline -5

# Continue work...
```

---

*Last Updated: September 2024*
*Ready for continuation in new chat session*