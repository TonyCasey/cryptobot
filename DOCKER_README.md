# CryptoBot Docker Setup

## Quick Start

### 1. Prerequisites
- Docker Desktop installed and running
- Docker Compose installed
- VS Code with Dev Containers extension (for development)

**Important:** Make sure Docker Desktop is running before executing docker commands!

### 2. Environment Setup
```bash
# Copy the environment template
cp .env.example .env

# Edit .env with your API keys and configuration
```

### 3. Running the Application

#### Check Docker Desktop
```bash
# First, ensure Docker Desktop is running!
docker version

# If you get an error, start Docker Desktop from Windows Start Menu
```

#### Production Mode (Linux Containers)
```bash
# Build and start all services
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down
```

#### Windows Containers Mode
```bash
# If using Windows containers
docker-compose -f docker-compose.windows.yml up -d
```

#### Local Development Mode (Using local SQL Server)
```bash
# If you have SQL Server Express installed locally
docker-compose -f docker-compose-local.yml up -d

# Access services:
# - API: http://localhost:5000
# - UI: http://localhost:3003
```

#### Development Mode with Full Stack
```bash
# Start with development profile (includes hot reload)
docker-compose --profile dev up -d

# Access services:
# - API: http://localhost:5000
# - UI: http://localhost:3003
# - SQL Server: localhost:1433
# - Redis: localhost:6379
```

### 4. VS Code Dev Container

1. Open VS Code
2. Install "Dev Containers" extension
3. Open Command Palette (F1)
4. Run "Dev Containers: Open Folder in Container"
5. Select the cryptobot folder
6. VS Code will rebuild and open in the container

#### Debugging in Dev Container
- .NET Framework: Use Mono debugger
- .NET Core API: F5 to start debugging
- Angular UI: Use Chrome DevTools

## Docker Commands

### Build Images
```bash
# Build main application
docker build -t cryptobot:latest .

# Build development image
docker build -f Dockerfile.dev -t cryptobot:dev .
```

### Database Management
```bash
# Connect to SQL Server
docker exec -it cryptobot-db /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P "CryptoBot@2024!"

# Backup database
docker exec cryptobot-db /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P "CryptoBot@2024!" \
  -Q "BACKUP DATABASE CryptoBot TO DISK = '/var/opt/mssql/backup/CryptoBot.bak'"
```

### Container Management
```bash
# View running containers
docker ps

# View all containers
docker ps -a

# View logs
docker logs cryptobot-app

# Enter container shell
docker exec -it cryptobot-app bash

# Clean up volumes
docker-compose down -v
```

## Architecture

### Services
- **cryptobot**: Main application (Windows container with IIS)
- **sqlserver**: SQL Server Express 2022
- **redis**: Redis cache for session management
- **cryptobot-api-dev**: Development API with hot reload
- **cryptobot-ui-dev**: Development UI with hot reload

### Volumes
- `sqlserver-data`: Persistent SQL Server data
- `redis-data`: Persistent Redis data
- `./logs`: Application logs (mapped to host)
- `./config`: Configuration files (mapped to host)

### Networks
- `cryptobot-network`: Bridge network for service communication

## Troubleshooting

### Docker Desktop Not Running
```bash
# Error: "The system cannot find the file specified"
# Solution: Start Docker Desktop from Windows Start Menu

# Verify Docker is running
docker version
```

### Port Conflicts
If ports are already in use, modify the port mappings in `docker-compose.yml`:
```yaml
ports:
  - "8080:80"  # Change 8080 to desired port
```

### SQL Server Connection Issues
```bash
# Check SQL Server logs
docker logs cryptobot-db

# Test connection
docker exec cryptobot-db /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P "CryptoBot@2024!" -Q "SELECT 1"
```

### Memory Issues
SQL Server Express is limited to 2GB RAM. If you need more:
1. Remove `MSSQL_MEMORY_LIMIT_MB` from docker-compose.yml
2. Change `MSSQL_PID=Express` to `MSSQL_PID=Developer`

### Build Failures
```bash
# Clean Docker cache
docker system prune -a

# Rebuild without cache
docker-compose build --no-cache
```

## Security Notes

- Change default passwords in production
- Use Docker secrets for sensitive data
- Enable TLS/SSL for production deployments
- Regularly update base images
- Scan images for vulnerabilities: `docker scan cryptobot:latest`