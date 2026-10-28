# Build stage: full SDK, restores and publishes the API.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Project files first: the restore layer is cached until a .csproj changes.
COPY global.json Directory.Build.props ./
COPY src/Comments.Domain/Comments.Domain.csproj src/Comments.Domain/
COPY src/Comments.Application/Comments.Application.csproj src/Comments.Application/
COPY src/Comments.Infrastructure/Comments.Infrastructure.csproj src/Comments.Infrastructure/
COPY src/Comments.Api/Comments.Api.csproj src/Comments.Api/
RUN dotnet restore src/Comments.Api/Comments.Api.csproj

COPY src/ src/
RUN dotnet publish src/Comments.Api/Comments.Api.csproj -c Release -o /app --no-restore

# Runtime stage: only the ASP.NET Core runtime and the published output.
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .

# The uploads volume inherits this owner, so the non-root app user can write to it.
RUN mkdir -p /app/uploads && chown "$APP_UID" /app/uploads
USER $APP_UID

EXPOSE 8080
ENTRYPOINT ["dotnet", "Comments.Api.dll"]
