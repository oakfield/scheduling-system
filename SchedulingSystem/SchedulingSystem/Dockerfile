# syntax=docker/dockerfile:1

# Build context is the repository root (see docker-compose.yml), so all
# COPY paths below are relative to the repo root, not this file's folder.

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080
USER $APP_UID

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["SchedulingSystem/SchedulingSystem/SchedulingSystem.csproj", "SchedulingSystem/SchedulingSystem/"]
RUN dotnet restore "SchedulingSystem/SchedulingSystem/SchedulingSystem.csproj"

COPY . .
WORKDIR /src/SchedulingSystem/SchedulingSystem
RUN dotnet build "SchedulingSystem.csproj" -c $BUILD_CONFIGURATION -o /app/build --no-restore

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SchedulingSystem.csproj" -c $BUILD_CONFIGURATION -o /app/publish --no-restore /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SchedulingSystem.dll"]
