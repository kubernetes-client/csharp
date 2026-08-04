FROM mcr.microsoft.com/dotnet/sdk:10.0

# Install .NET 8 and .NET 9 SDKs side-by-side so the container supports net8.0, net9.0 and net10.0 builds.
RUN set -eux; \
    apt-get update; \
    apt-get install -y --no-install-recommends curl ca-certificates; \
    rm -rf /var/lib/apt/lists/*; \
    curl -fsSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh; \
    chmod +x /tmp/dotnet-install.sh; \
    /tmp/dotnet-install.sh --channel 9.0 --quality ga --install-dir /usr/share/dotnet --skip-non-versioned-files; \
    /tmp/dotnet-install.sh --channel 8.0 --quality ga --install-dir /usr/share/dotnet --skip-non-versioned-files; \
    rm -f /tmp/dotnet-install.sh
