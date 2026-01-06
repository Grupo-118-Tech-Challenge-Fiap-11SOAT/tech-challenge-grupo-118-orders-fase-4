FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base

WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/Adapters/Driving/Orders.API/Orders.API.csproj", "Adapters/Driving/Orders.API/"]
COPY ["src/Core/Application/Application.csproj", "Core/Application/"]
COPY ["src/Core/Domain/Domain.csproj", "Core/Domain/"]
COPY ["src/Adapters/Driven/Infra.Database.SqlServer/Infra.Database.SqlServer.csproj", "Adapters/Driven/Infra.Database.SqlServer/"]

RUN dotnet restore "src/Adapters/Driving/Orders.API/Orders.API.csproj"

COPY . .
WORKDIR "/src/src/Adapters/Driving/Orders.API"
RUN dotnet build "./Orders.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Orders.API.csproj" \
     -c $BUILD_CONFIGURATION \
     -o /app/publish \
     /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

USER $APP_UID

ENTRYPOINT ["dotnet", "Orders.API.dll"]
