# Multi-stage build for CryptoBot solution

# Stage 1: Build .NET Framework projects
FROM mcr.microsoft.com/dotnet/framework/sdk:4.8 AS dotnet-build
WORKDIR /src

# Copy solution and project files
COPY *.sln .
COPY nuget.config* ./
COPY CryptoBot.Core/*.csproj ./CryptoBot.Core/
COPY CryptoBot.Model/*.csproj ./CryptoBot.Model/
COPY CryptoBot.Database/*.csproj ./CryptoBot.Database/
COPY CryptoBot.ExchangeEngine/*.csproj ./CryptoBot.ExchangeEngine/
COPY CryptoBot.IndicatorEngine/*.csproj ./CryptoBot.IndicatorEngine/
COPY CryptoBot.SafetyEngine/*.csproj ./CryptoBot.SafetyEngine/
COPY CryptoBot.BackTester/*.csproj ./CryptoBot.BackTester/
COPY CryptoBot.Tests/*.csproj ./CryptoBot.Tests/

# Restore NuGet packages
RUN nuget restore CryptoBot.sln

# Copy source code
COPY CryptoBot.Core/ ./CryptoBot.Core/
COPY CryptoBot.Model/ ./CryptoBot.Model/
COPY CryptoBot.Database/ ./CryptoBot.Database/
COPY CryptoBot.ExchangeEngine/ ./CryptoBot.ExchangeEngine/
COPY CryptoBot.IndicatorEngine/ ./CryptoBot.IndicatorEngine/
COPY CryptoBot.SafetyEngine/ ./CryptoBot.SafetyEngine/
COPY CryptoBot.BackTester/ ./CryptoBot.BackTester/
COPY CryptoBot.Tests/ ./CryptoBot.Tests/

# Build the solution
RUN msbuild CryptoBot.sln /p:Configuration=Release /p:Platform="Any CPU"

# Stage 2: Build ASP.NET Core API
FROM mcr.microsoft.com/dotnet/core/sdk:2.1 AS api-build
WORKDIR /src

# Copy API project files
COPY CryptoBot.Api/*.csproj ./CryptoBot.Api/

# Restore packages
WORKDIR /src/CryptoBot.Api
RUN dotnet restore

# Copy API source code
COPY CryptoBot.Api/ .

# Copy Model project (needed for API)
COPY CryptoBot.Model/ /src/CryptoBot.Model/

# Build API
RUN dotnet publish -c Release -o /app/api

# Stage 3: Build Angular UI
FROM node:14-alpine AS ui-build
WORKDIR /app

# Copy package files
COPY CryptoBot.UI/package*.json ./

# Install dependencies
RUN npm ci

# Copy UI source code
COPY CryptoBot.UI/ .

# Build UI
RUN npm run build

# Stage 4: Runtime image
FROM mcr.microsoft.com/dotnet/framework/runtime:4.8 AS runtime
WORKDIR /app

# Install IIS and ASP.NET Core Module
RUN powershell -Command \
    Add-WindowsFeature Web-Server; \
    Add-WindowsFeature Web-Asp-Net45; \
    Invoke-WebRequest -UseBasicParsing -Uri "https://download.visualstudio.microsoft.com/download/pr/5c8e5c58-0d6e-4fb3-b6b3-bcc0b023b4e8/7e886d60729949d08e65b1e05b5717e9/dotnetcore-2.1.30-windowshosting.exe" -OutFile dotnet-hosting.exe; \
    Start-Process ".\dotnet-hosting.exe" -ArgumentList "/quiet" -Wait; \
    Remove-Item -Force dotnet-hosting.exe

# Copy built .NET Framework assemblies
COPY --from=dotnet-build /src/CryptoBot.Core/bin/Release/ ./
COPY --from=dotnet-build /src/CryptoBot.ExchangeEngine/bin/Release/ ./
COPY --from=dotnet-build /src/CryptoBot.Model/bin/Release/ ./
COPY --from=dotnet-build /src/CryptoBot.Database/bin/Release/ ./
COPY --from=dotnet-build /src/CryptoBot.IndicatorEngine/bin/Release/ ./
COPY --from=dotnet-build /src/CryptoBot.SafetyEngine/bin/Release/ ./
COPY --from=dotnet-build /src/CryptoBot.BackTester/bin/Release/ ./

# Copy API
COPY --from=api-build /app/api ./api/

# Copy UI build
COPY --from=ui-build /app/dist ./wwwroot/

# Configure IIS
RUN powershell -Command \
    Import-Module WebAdministration; \
    New-Website -Name 'CryptoBot' -Port 80 -PhysicalPath 'C:\app\wwwroot'; \
    New-WebApplication -Name 'api' -Site 'CryptoBot' -PhysicalPath 'C:\app\api'

# Expose ports
EXPOSE 80
EXPOSE 443

# Start IIS
ENTRYPOINT ["powershell", "-Command", "& {Start-Service W3SVC; while ($true) { Start-Sleep -Seconds 3600 }}"]