# Development container with all necessary tools for CryptoBot
FROM mcr.microsoft.com/devcontainers/base:ubuntu-22.04

# Install essential tools
RUN apt-get update && apt-get install -y \
    wget \
    curl \
    gnupg \
    lsb-release \
    ca-certificates \
    apt-transport-https \
    software-properties-common \
    build-essential \
    git \
    vim \
    nano \
    unzip \
    zip \
    && rm -rf /var/lib/apt/lists/*

# Install Mono for .NET Framework support
RUN apt-get update && apt-get install -y \
    mono-complete \
    mono-dbg \
    referenceassemblies-pcl \
    mono-xsp4 \
    && rm -rf /var/lib/apt/lists/*

# Install .NET Core 2.1 SDK
RUN wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && rm packages-microsoft-prod.deb \
    && apt-get update \
    && apt-get install -y dotnet-sdk-2.1 \
    && rm -rf /var/lib/apt/lists/*

# Install .NET 6 SDK (for tooling compatibility)
RUN apt-get update && apt-get install -y \
    dotnet-sdk-6.0 \
    && rm -rf /var/lib/apt/lists/*

# Install Node.js 14 for Angular UI
RUN curl -fsSL https://deb.nodesource.com/setup_14.x | bash - \
    && apt-get install -y nodejs \
    && npm install -g npm@latest \
    && npm install -g @angular/cli@4.3.1 \
    && npm install -g webpack \
    && rm -rf /var/lib/apt/lists/*

# Install SQL Server tools
RUN curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add - \
    && curl https://packages.microsoft.com/config/ubuntu/22.04/prod.list | tee /etc/apt/sources.list.d/msprod.list \
    && apt-get update \
    && ACCEPT_EULA=Y apt-get install -y mssql-tools unixodbc-dev \
    && rm -rf /var/lib/apt/lists/*

# Add SQL Server tools to PATH
ENV PATH="$PATH:/opt/mssql-tools/bin"

# Install NuGet CLI
RUN wget https://dist.nuget.org/win-x86-commandline/latest/nuget.exe -O /usr/local/bin/nuget.exe \
    && echo '#!/bin/bash\nmono /usr/local/bin/nuget.exe "$@"' > /usr/local/bin/nuget \
    && chmod +x /usr/local/bin/nuget

# Install MSBuild (via Mono)
RUN ln -s /usr/bin/xbuild /usr/local/bin/msbuild

# Create non-root user
ARG USERNAME=vscode
ARG USER_UID=1000
ARG USER_GID=$USER_UID

RUN groupadd --gid $USER_GID $USERNAME \
    && useradd --uid $USER_UID --gid $USER_GID -m $USERNAME \
    && apt-get update \
    && apt-get install -y sudo \
    && echo $USERNAME ALL=\(root\) NOPASSWD:ALL > /etc/sudoers.d/$USERNAME \
    && chmod 0440 /etc/sudoers.d/$USERNAME \
    && rm -rf /var/lib/apt/lists/*

# Set up workspace directory
RUN mkdir -p /workspace && chown -R $USERNAME:$USERNAME /workspace

# Switch to non-root user
USER $USERNAME

# Set environment variables
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1 \
    DOTNET_USE_POLLING_FILE_WATCHER=1 \
    NUGET_XMLDOC_MODE=skip \
    PATH="/home/${USERNAME}/.dotnet/tools:${PATH}"

# Install .NET global tools
RUN dotnet tool install --global dotnet-ef --version 2.1.* \
    && dotnet tool install --global dotnet-aspnet-codegenerator --version 2.1.*

WORKDIR /workspace