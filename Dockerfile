## Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and restore
COPY ./Booking.sln ./Booking.sln
COPY ./src/ ./src/
COPY ./tests/ ./tests/

RUN dotnet restore ./Booking.sln

# Publish API
WORKDIR /src/src/Booking.Api
RUN dotnet publish -c Release -o /app/publish

## Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Default ASP.NET Core environment and URL (overridable via env in compose/VPS)
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# Logs directory for Serilog file sink
RUN mkdir -p /app/logs

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT [\"dotnet\", \"Booking.Api.dll\"]

