ARG PROJECT=App

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG PROJECT
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY . .
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

ENTRYPOINT sh -c "dotnet ${PROJECT_DLL}"