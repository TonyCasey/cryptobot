# .NET 8 Migration Scripts

This document contains the new SDK-style project files for each project in the solution.

---

## 1. CryptoBot.Model (✅ Created as CryptoBot.Model_NET8.csproj)

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <RootNamespace>CryptoBot.Model</RootNamespace>
    <AssemblyName>CryptoBot.Model</AssemblyName>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <LangVersion>latest</LangVersion>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="AutoMapper" Version="13.0.1" />
    <PackageReference Include="System.ComponentModel.Annotations" Version="5.0.0" />
    <PackageReference Include="Microsoft.Extensions.Configuration.Abstractions" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.Options" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="8.0.0" />
    <PackageReference Include="Microsoft.Net.Http.Headers" Version="8.0.0" />
  </ItemGroup>

</Project>
```

---

## 2. CryptoBot.Database

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <RootNamespace>CryptoBot.Database</RootNamespace>
    <AssemblyName>CryptoBot.Database</AssemblyName>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <!-- EF Core 8 instead of EF 6 -->
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="AutoMapper" Version="13.0.1" />
    <PackageReference Include="NLog" Version="5.2.8" />
    <PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="8.0.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\CryptoBot.Model\CryptoBot.Model.csproj" />
  </ItemGroup>

</Project>
```

---

## 3. CryptoBot.ExchangeEngine

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <RootNamespace>CryptoBot.ExchangeEngine</RootNamespace>
    <AssemblyName>CryptoBot.ExchangeEngine</AssemblyName>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
    <PackageReference Include="NLog" Version="5.2.8" />
    <PackageReference Include="RestSharp" Version="110.2.0" />
    <!-- WebSocketSharp replacement -->
    <PackageReference Include="System.Net.WebSockets.Client" Version="4.3.2" />
    <!-- Updated GDAX/Coinbase libraries if available, or custom implementation -->
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\CryptoBot.Model\CryptoBot.Model.csproj" />
  </ItemGroup>

</Project>
```

---

## 4. CryptoBot.IndicatorEngine

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <RootNamespace>CryptoBot.IndicatorEngine</RootNamespace>
    <AssemblyName>CryptoBot.IndicatorEngine</AssemblyName>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="NLog" Version="5.2.8" />
    <!-- TA-Lib may need special handling or replacement -->
    <PackageReference Include="TALib.NETCore" Version="0.4.5" />
    <!-- Updated Trady libraries -->
    <PackageReference Include="Trady.Analysis" Version="3.2.1" />
    <PackageReference Include="Trady.Core" Version="3.2.1" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\CryptoBot.Model\CryptoBot.Model.csproj" />
  </ItemGroup>

</Project>
```

---

## 5. CryptoBot.SafetyEngine

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <RootNamespace>CryptoBot.SafetyEngine</RootNamespace>
    <AssemblyName>CryptoBot.SafetyEngine</AssemblyName>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="NLog" Version="5.2.8" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\CryptoBot.Model\CryptoBot.Model.csproj" />
  </ItemGroup>

</Project>
```

---

## 6. CryptoBot.Core

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <RootNamespace>CryptoBot.Core</RootNamespace>
    <AssemblyName>CryptoBot.Core</AssemblyName>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="AutoMapper" Version="13.0.1" />
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
    <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
    <PackageReference Include="NLog" Version="5.2.8" />
    <PackageReference Include="Telegram.Bot" Version="19.0.0" />
    <PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="8.0.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\CryptoBot.Database\CryptoBot.Database.csproj" />
    <ProjectReference Include="..\CryptoBot.ExchangeEngine\CryptoBot.ExchangeEngine.csproj" />
    <ProjectReference Include="..\CryptoBot.IndicatorEngine\CryptoBot.IndicatorEngine.csproj" />
    <ProjectReference Include="..\CryptoBot.Model\CryptoBot.Model.csproj" />
    <ProjectReference Include="..\CryptoBot.SafetyEngine\CryptoBot.SafetyEngine.csproj" />
  </ItemGroup>

</Project>
```

---

## 7. CryptoBot.BackTester

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <RootNamespace>CryptoBot.BackTester</RootNamespace>
    <AssemblyName>CryptoBot.BackTester</AssemblyName>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="CsvHelper" Version="30.0.1" />
    <PackageReference Include="NLog" Version="5.2.8" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\CryptoBot.Core\CryptoBot.Core.csproj" />
    <ProjectReference Include="..\CryptoBot.Model\CryptoBot.Model.csproj" />
  </ItemGroup>

</Project>
```

---

## 8. CryptoBot.Console

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <RootNamespace>CryptoBot</RootNamespace>
    <AssemblyName>CryptoBot</AssemblyName>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <ApplicationIcon>bot.ico</ApplicationIcon>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Autofac" Version="7.1.0" />
    <PackageReference Include="AutoMapper" Version="13.0.1" />
    <PackageReference Include="CsvHelper" Version="30.0.1" />
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.Caching.Memory" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.Hosting" Version="8.0.0" />
    <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
    <PackageReference Include="NLog" Version="5.2.8" />
    <PackageReference Include="NLog.Extensions.Logging" Version="5.3.8" />
    <PackageReference Include="RestSharp" Version="110.2.0" />
    <PackageReference Include="Telegram.Bot" Version="19.0.0" />
    <!-- TA-Lib replacement -->
    <PackageReference Include="TALib.NETCore" Version="0.4.5" />
    <!-- Updated Trady -->
    <PackageReference Include="Trady.Analysis" Version="3.2.1" />
    <PackageReference Include="Trady.Core" Version="3.2.1" />
    <PackageReference Include="Trady.Importer" Version="3.2.1" />
    <!-- API clients that need updating -->
    <PackageReference Include="YahooFinanceApi" Version="2.3.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\CryptoBot.Core\CryptoBot.Core.csproj" />
    <ProjectReference Include="..\CryptoBot.Database\CryptoBot.Database.csproj" />
    <ProjectReference Include="..\CryptoBot.ExchangeEngine\CryptoBot.ExchangeEngine.csproj" />
    <ProjectReference Include="..\CryptoBot.IndicatorEngine\CryptoBot.IndicatorEngine.csproj" />
    <ProjectReference Include="..\CryptoBot.Model\CryptoBot.Model.csproj" />
  </ItemGroup>

  <ItemGroup>
    <Content Include="bot.ico" />
  </ItemGroup>

  <ItemGroup>
    <None Update="appsettings.json">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
    <None Update="appsettings.Development.json">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
    <None Update="appsettings.Production.json">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
  </ItemGroup>

</Project>
```

---

## 9. CryptoBot.Tests

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <RootNamespace>CryptoBot.Tests</RootNamespace>
    <AssemblyName>CryptoBot.Tests</AssemblyName>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <!-- Option 1: Continue with MSTest -->
    <PackageReference Include="MSTest.TestAdapter" Version="3.1.1" />
    <PackageReference Include="MSTest.TestFramework" Version="3.1.1" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
    
    <!-- Option 2: Migrate to xUnit (recommended) -->
    <!-- <PackageReference Include="xunit" Version="2.6.6" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.6">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference> -->
    
    <PackageReference Include="Moq" Version="4.20.70" />
    <PackageReference Include="AutoMapper" Version="13.0.1" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="8.0.0" />
    <PackageReference Include="NLog" Version="5.2.8" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\CryptoBot.Core\CryptoBot.Core.csproj" />
    <ProjectReference Include="..\CryptoBot.Database\CryptoBot.Database.csproj" />
    <ProjectReference Include="..\CryptoBot.ExchangeEngine\CryptoBot.ExchangeEngine.csproj" />
    <ProjectReference Include="..\CryptoBot.IndicatorEngine\CryptoBot.IndicatorEngine.csproj" />
    <ProjectReference Include="..\CryptoBot.Model\CryptoBot.Model.csproj" />
  </ItemGroup>

</Project>
```

---

## 10. CryptoBot.Api (ASP.NET Core 2.0 → 8.0)

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <RootNamespace>Api.CryptoBot</RootNamespace>
    <AssemblyName>Api.CryptoBot</AssemblyName>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Autofac.Extensions.DependencyInjection" Version="8.0.0" />
    <PackageReference Include="AutoMapper" Version="13.0.1" />
    <PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="12.0.1" />
    <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="8.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
    <PackageReference Include="Asp.Versioning.Mvc" Version="8.0.0" />
    <PackageReference Include="Asp.Versioning.Mvc.ApiExplorer" Version="8.0.0" />
    <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\CryptoBot.Core\CryptoBot.Core.csproj" />
    <ProjectReference Include="..\CryptoBot.ExchangeEngine\CryptoBot.ExchangeEngine.csproj" />
    <ProjectReference Include="..\CryptoBot.Model\CryptoBot.Model.csproj" />
  </ItemGroup>

</Project>
```

---

## Migration Steps for Each Project

### Step 1: Backup Original
```bash
copy "ProjectName.csproj" "ProjectName.csproj.backup"
```

### Step 2: Delete Old Files
- Delete `packages.config`
- Delete `Properties\AssemblyInfo.cs` (now auto-generated)
- Delete `App.config` (replace with appsettings.json)

### Step 3: Replace Project File
Replace the old `.csproj` with the new SDK-style version above

### Step 4: Create appsettings.json (for executable projects)
```json
{
  "ConnectionStrings": {
    "CryptoBot": "Server=(localdb)\\mssqllocaldb;Database=CryptoBot;Trusted_Connection=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AppSettings": {
    // Move settings from app.config here
  }
}
```

### Step 5: Update Code
- Update using statements for new namespaces
- Update EF6 code to EF Core
- Update configuration reading code
- Fix any breaking API changes

### Step 6: Build and Test
```bash
dotnet restore
dotnet build
dotnet test
```

---

## Global.json (Optional - Pin SDK Version)

Create in solution root:
```json
{
  "sdk": {
    "version": "8.0.100",
    "rollForward": "latestMinor"
  }
}
```

---

## New Solution File Approach (Optional)

Create a new solution file for .NET 8:
```bash
dotnet new sln -n CryptoBot.NET8
dotnet sln add CryptoBot.Model/CryptoBot.Model.csproj
dotnet sln add CryptoBot.Database/CryptoBot.Database.csproj
# ... add all projects
```

This allows parallel development during migration.