# =====================================================================
# TerraPulse API - Dockerfile multi-stage (.NET 8)
# Contexto de build: raiz do repositorio
# =====================================================================

# ---------- Etapa 1: build e publish ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copia apenas o .csproj primeiro para aproveitar o cache do "restore"
COPY src/TerraPulse.Api/TerraPulse.Api.csproj src/TerraPulse.Api/
RUN dotnet restore src/TerraPulse.Api/TerraPulse.Api.csproj

# Copia o restante do codigo e publica em modo Release
COPY src/ src/
RUN dotnet publish src/TerraPulse.Api/TerraPulse.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# ---------- Etapa 2: runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Versao da imagem (SHA do commit), informada pelo pipeline
ARG APP_VERSION=local
ENV APP_VERSION=${APP_VERSION} \
    ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_RUNNING_IN_CONTAINER=true

LABEL org.opencontainers.image.title="terrapulse-api" \
      org.opencontainers.image.description="TerraPulse - API ESG para Cidades Inteligentes" \
      org.opencontainers.image.version="${APP_VERSION}"

COPY --from=build /app/publish .

# Executa com usuario nao-root (ja existente na imagem oficial do .NET 8)
USER $APP_UID

EXPOSE 8080

ENTRYPOINT ["dotnet", "TerraPulse.Api.dll"]
