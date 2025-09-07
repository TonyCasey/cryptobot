# .NET 8 Migration Guide for CryptoBot

## ⚠️ CRITICAL: Pre-Migration Checklist

### BEFORE YOU BEGIN - MANDATORY STEPS:
1. **CREATE A FULL BACKUP** of your entire solution
   ```bash
   git checkout -b dotnet8-migration-backup
   git add .
   git commit -m "Backup before .NET 8 migration"
   ```

2. **Install Prerequisites:**
   - [ ] .NET 8 SDK (https://dotnet.microsoft.com/download/dotnet/8.0)
   - [ ] Visual Studio 2022 (v17.8 or later)
   - [ ] .NET Upgrade Assistant:
   ```bash
   dotnet tool install -g upgrade-assistant
   ```

3. **Document Current State:**
   - [ ] Run all tests and document passing count
   - [ ] Note any existing build warnings
   - [ ] Document current deployment process

---

## 📋 Migration Roadmap

### Current Architecture
- **Framework**: .NET Framework 4.5.2 / 4.6.2
- **API Project**: ASP.NET Core 2.0 (easiest to migrate)
- **ORM**: Entity Framework 6.2.0
- **Test Framework**: MSTest
- **UI**: Angular 4.3.1 (separate, no migration needed)

### Target Architecture
- **Framework**: .NET 8.0 (LTS)
- **API Project**: ASP.NET Core 8.0
- **ORM**: Entity Framework Core 8.0
- **Test Framework**: MSTest 3.x or xUnit 2.6+

---

## 🔄 Migration Order (Dependencies Matter!)

### Phase 1: Base Projects (No Dependencies)
1. **CryptoBot.Model**
   - Simple domain models
   - No external dependencies
   - Easiest starting point

### Phase 2: Data Layer
2. **CryptoBot.Database**
   - ⚠️ **MAJOR CHANGE**: EF6 to EF Core 8
   - Requires migration strategy rewrite
   - Connection string updates

### Phase 3: Service Layers
3. **CryptoBot.ExchangeEngine**
4. **CryptoBot.IndicatorEngine** 
5. **CryptoBot.SafetyEngine**

### Phase 4: Core Business Logic
6. **CryptoBot.Core**
   - Depends on all service layers
   - May need significant updates

### Phase 5: Applications
7. **CryptoBot.BackTester**
8. **CryptoBot.Console** (Main App)
9. **CryptoBot.Tests**
10. **CryptoBot.Api** (Simplest - already ASP.NET Core)

---

## 🛠️ Step-by-Step Migration Process

### For Each Project:

#### Step 1: Convert Project File
```xml
<!-- OLD (.NET Framework) -->
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="15.0" xmlns="...">
  <PropertyGroup>
    <TargetFrameworkVersion>v4.5.2</TargetFrameworkVersion>
  </PropertyGroup>
  <!-- Complex structure -->
</Project>

<!-- NEW (.NET 8 SDK-style) -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
</Project>
```

#### Step 2: Migrate Package References
Remove `packages.config` and add to `.csproj`:
```xml
<ItemGroup>
  <PackageReference Include="PackageName" Version="x.x.x" />
</ItemGroup>
```

---

## 📦 Package Migration Matrix

| Current Package | Version | .NET 8 Replacement | Version | Breaking Changes |
|----------------|---------|-------------------|---------|-----------------|
| **Entity Framework** | 6.2.0 | Microsoft.EntityFrameworkCore | 8.0.x | ⚠️ MAJOR |
| | | Microsoft.EntityFrameworkCore.SqlServer | 8.0.x | |
| | | Microsoft.EntityFrameworkCore.Tools | 8.0.x | |
| **AutoMapper** | 6.2.2 | AutoMapper | 13.0.x | Minor |
| **Autofac** | 4.6.2 | Autofac | 7.1.x | Minor |
| **NLog** | 4.4.12 | NLog | 5.2.x | Config changes |
| **Newtonsoft.Json** | 10.0.3 | System.Text.Json | Built-in | API differences |
| **MSTest** | 1.2.0 | MSTest.TestFramework | 3.1.x | Minor |
| **Telegram.Bot** | 13.4.0 | Telegram.Bot | 19.0.x | API changes |
| **RestSharp** | 105.2.3 | RestSharp | 110.2.x | Major API changes |
| **WebSocketSharp** | 1.0.3 | System.Net.WebSockets | Built-in | ⚠️ REPLACEMENT |
| **Flurl.Http** | 1.1.1 | Flurl.Http | 4.0.x | Minor |
| **CsvHelper** | 2.16.3 | CsvHelper | 30.0.x | Breaking changes |

---

## ⚠️ Major Breaking Changes & Solutions

### 1. Entity Framework 6 → EF Core 8

#### Migration Code Changes:
```csharp
// OLD (EF6)
public class CryptoBotDbContext : DbContext
{
    public CryptoBotDbContext() : base("name=ConnectionString") { }
    
    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        // EF6 configuration
    }
}

// NEW (EF Core 8)
public class CryptoBotDbContext : DbContext
{
    public CryptoBotDbContext(DbContextOptions<CryptoBotDbContext> options) 
        : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // EF Core configuration
    }
}
```

#### Migration Strategy:
1. **Export existing data**
2. **Recreate migrations** for EF Core
3. **Update LINQ queries** (some may break)
4. **Update connection string handling**

### 2. Configuration System

#### OLD (app.config):
```xml
<configuration>
  <connectionStrings>
    <add name="CryptoBot" connectionString="..." />
  </connectionStrings>
  <appSettings>
    <add key="ApiKey" value="..." />
  </appSettings>
</configuration>
```

#### NEW (appsettings.json):
```json
{
  "ConnectionStrings": {
    "CryptoBot": "..."
  },
  "AppSettings": {
    "ApiKey": "..."
  }
}
```

#### Code Changes:
```csharp
// OLD
var apiKey = ConfigurationManager.AppSettings["ApiKey"];

// NEW
public class MyService
{
    private readonly IConfiguration _configuration;
    
    public MyService(IConfiguration configuration)
    {
        _configuration = configuration;
        var apiKey = _configuration["AppSettings:ApiKey"];
    }
}
```

### 3. WebSocket Replacement

#### OLD (WebSocketSharp):
```csharp
using WebSocketSharp;

var ws = new WebSocket("wss://example.com");
ws.OnMessage += (sender, e) => { };
ws.Connect();
```

#### NEW (System.Net.WebSockets):
```csharp
using System.Net.WebSockets;

var ws = new ClientWebSocket();
await ws.ConnectAsync(new Uri("wss://example.com"), CancellationToken.None);
// Different API for message handling
```

### 4. Dependency Injection

#### Program.cs Changes:
```csharp
// NEW (.NET 8 minimal API style)
var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddDbContext<CryptoBotDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CryptoBot")));

builder.Services.AddScoped<ITradeBot, TradeBot>();
builder.Services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();
```

---

## 🔧 Specific Project Migration Notes

### CryptoBot.Api (ASP.NET Core 2.0 → 8.0)
```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
  
  <ItemGroup>
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
    <PackageReference Include="Microsoft.AspNetCore.Mvc.Versioning" Version="5.1.0" />
  </ItemGroup>
</Project>
```

### CryptoBot.Console
- Remove ClickOnce publishing settings
- Update to top-level program style (optional)
- Replace System.Configuration with Microsoft.Extensions.Configuration

### CryptoBot.Tests
Consider migrating to xUnit for better .NET Core support:
```xml
<ItemGroup>
  <PackageReference Include="xunit" Version="2.6.x" />
  <PackageReference Include="xunit.runner.visualstudio" Version="2.5.x" />
  <PackageReference Include="Moq" Version="4.20.x" />
</ItemGroup>
```

---

## 📝 Post-Migration Checklist

- [ ] All projects compile without errors
- [ ] All tests pass
- [ ] Connection strings work
- [ ] Configuration loads correctly
- [ ] WebSocket connections functional
- [ ] API endpoints respond correctly
- [ ] Database migrations run successfully
- [ ] Performance benchmarks acceptable
- [ ] Deployment pipeline updated
- [ ] Documentation updated

---

## 🚀 Deployment Changes

### New Publishing Command:
```bash
# Build for production
dotnet publish -c Release -r win-x64 --self-contained

# Or for framework-dependent deployment
dotnet publish -c Release
```

### Docker Support (Optional):
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["CryptoBot.Api/CryptoBot.Api.csproj", "CryptoBot.Api/"]
RUN dotnet restore "CryptoBot.Api/CryptoBot.Api.csproj"
COPY . .
RUN dotnet build "CryptoBot.Api/CryptoBot.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CryptoBot.Api/CryptoBot.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CryptoBot.Api.dll"]
```

---

## 🆘 Troubleshooting Guide

### Common Issues:

1. **"Type or namespace not found"**
   - Update package references
   - Check for removed APIs in .NET 8

2. **EF Core Migration Failures**
   - Recreate migrations from scratch
   - Check SQL compatibility

3. **Configuration Not Loading**
   - Ensure appsettings.json is copied to output
   - Check IConfiguration injection

4. **WebSocket Connection Issues**
   - Verify TLS 1.2+ support
   - Check firewall rules

---

## 📚 Resources

- [.NET Upgrade Assistant Documentation](https://docs.microsoft.com/dotnet/core/porting/upgrade-assistant-overview)
- [EF Core Migration from EF6](https://docs.microsoft.com/ef/efcore-and-ef6/porting/)
- [Breaking Changes in .NET 8](https://docs.microsoft.com/dotnet/core/compatibility/8.0)
- [ASP.NET Core Migration](https://docs.microsoft.com/aspnet/core/migration/22-to-30)

---

## ⏱️ Estimated Timeline

| Phase | Duration | Complexity |
|-------|----------|------------|
| Backup & Setup | 1 day | Low |
| Model Project | 2 hours | Low |
| Database/EF Migration | 1 week | High |
| Service Layers | 3 days | Medium |
| Core Project | 2 days | Medium |
| Applications | 3 days | Medium |
| Testing & Validation | 1 week | High |
| **Total** | **3-4 weeks** | |

---

## 🎯 Success Criteria

✅ All projects on .NET 8.0
✅ All tests passing
✅ Performance equal or better
✅ No data loss during migration
✅ Deployment successful
✅ Documentation updated

---

*Last Updated: September 2024*
*Migration Guide Version: 1.0*