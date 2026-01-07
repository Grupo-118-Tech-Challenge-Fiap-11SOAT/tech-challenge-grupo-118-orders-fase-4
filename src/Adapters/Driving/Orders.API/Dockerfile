FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base

WORKDIR /app
RUN apk update && apk add --no-cache wget icu-libs icu-data-full tzdata ttf-opensans
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
RUN ln -s /usr/share/zoneinfo/America/Sao_Paulo /etc/localtime
ENV LC_ALL=pt_BR.UTF-8 \
    LANG=pt_BR.UTF-8
USER $APP_UID

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Adapters/Driving/Orders.API/Orders.API.csproj", "Adapters/Driving/Orders.API/"]
COPY ["src/Core/Application/Application.csproj", "Core/Application/"]
COPY ["src/Core/Domain/Domain.csproj", "Core/Domain/"]
COPY ["src/Adapters/Driven/Infra.Database.SqlServer/Infra.Database.SqlServer.csproj", "Adapters/Driven/Infra.Database.SqlServer/"]

RUN dotnet restore "src/Adapters/Driving/Orders.API/Orders.API.csproj"

COPY src/ .
WORKDIR "/src/src/Adapters/Driving/Orders.API"
RUN dotnet build "./Orders.API.csproj" -c Release -o /app --no-restore

FROM build AS publish
RUN dotnet publish "./Orders.API.csproj" -c Release -o /app --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Orders.API.dll"]
