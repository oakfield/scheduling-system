# syntax=docker/dockerfile:1

# Build context is the repository root (see docker-compose.yml), so all
# COPY paths below are relative to the repo root, not this file's folder.

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080

# Created (as root, before dropping to the non-root user below) and pre-owned by
# $APP_UID so that when docker-compose mounts an empty named volume here, Docker's
# copy-in-on-first-mount behavior gives the volume these permissions instead of
# defaulting the mount point to root, which the app couldn't then write to.
RUN mkdir -p DataProtection-Keys && chown $APP_UID DataProtection-Keys

USER $APP_UID

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Deliberately not split into a restore-only layer on just the .csproj (the usual
# Docker-caching trick): restoring before the rest of the source is copied, then
# publishing with --no-restore afterward, left the static web assets manifest missing
# the framework's own blazor.web.js — an incremental-build staleness issue from doing
# restore and publish as separate MSBuild invocations across layers. A single publish
# (implicit restore) avoids it, at the cost of re-restoring on every source change.
COPY . .
WORKDIR /src/SchedulingSystem/SchedulingSystem
RUN dotnet publish "SchedulingSystem.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SchedulingSystem.dll"]
