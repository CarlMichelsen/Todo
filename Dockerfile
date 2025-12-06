ARG PROJECT=App
ARG FRONTEND_PROJECT=Frontend

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

# Frontend build stage - only runs if FRONTEND_PROJECT is defined
FROM oven/bun:1-alpine AS frontend-build
ARG FRONTEND_PROJECT

WORKDIR /src
COPY . .

WORKDIR /frontend

# Build frontend and standardize output location
RUN if [ -n "$FRONTEND_PROJECT" ]; then \
        echo "Building frontend: ${FRONTEND_PROJECT}"; \
        if [ ! -d "/src/${FRONTEND_PROJECT}" ]; then \
            echo "ERROR: Frontend project directory /src/${FRONTEND_PROJECT} does not exist"; \
            exit 1; \
        fi; \
        cp -r /src/${FRONTEND_PROJECT}/. /frontend/; \
        bun install; \
        bun run build; \
        mkdir -p /frontend/output; \
        if [ -d "dist" ]; then \
            cp -r dist/* /frontend/output/; \
            echo "Frontend build complete - copied from dist/"; \
        elif [ -d ".svelte-kit/output/client" ]; then \
            cp -r .svelte-kit/output/client/* /frontend/output/; \
            echo "Frontend build complete - copied from .svelte-kit/output/client/"; \
        else \
            echo "ERROR: bun run build did not create dist/ or .svelte-kit/output/client/ directory"; \
            exit 1; \
        fi; \
    else \
        mkdir -p /frontend/output; \
    fi

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG PROJECT
ARG FRONTEND_PROJECT
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY . .

# Create wwwroot directory
RUN mkdir -p "${PROJECT}/wwwroot"

# Copy frontend output
COPY --from=frontend-build /frontend/output/. /src/${PROJECT}/wwwroot/

# Verify frontend was copied if FRONTEND_PROJECT was specified
RUN if [ -n "$FRONTEND_PROJECT" ]; then \
        if [ -z "$(ls -A ${PROJECT}/wwwroot/)" ]; then \
            echo "ERROR: FRONTEND_PROJECT was set but wwwroot is empty"; \
            exit 1; \
        fi; \
        echo "Frontend successfully copied to wwwroot:"; \
        ls -la "${PROJECT}/wwwroot/" | head -20; \
    fi

RUN dotnet restore "${PROJECT}/${PROJECT}.csproj"

WORKDIR "/src/${PROJECT}"
RUN dotnet build "./${PROJECT}.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG PROJECT
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./${PROJECT}.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
ARG PROJECT
ENV PROJECT_DLL="${PROJECT}.dll"
WORKDIR /app
COPY --from=publish /app/publish .

HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1
    
ENV ASPNETCORE_URLS=http://+:8080 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

ENTRYPOINT ["sh", "-c", "dotnet ${PROJECT_DLL}"]