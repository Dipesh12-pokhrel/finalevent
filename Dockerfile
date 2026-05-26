# Railway-compatible Dockerfile (build context = repo root)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY EventManagement/EventManagement.csproj ./EventManagement/
RUN dotnet restore "EventManagement/EventManagement.csproj"

# Copy everything and publish
COPY EventManagement/. ./EventManagement/
WORKDIR /src/EventManagement
RUN dotnet publish "EventManagement.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Railway injects PORT env var; fall back to 8080
ENV ASPNETCORE_URLS=http://+:${PORT:-8080}
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080
ENTRYPOINT ["dotnet", "EventManagement.dll"]
