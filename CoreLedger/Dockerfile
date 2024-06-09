FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CoreLedger/CoreLedger.fsproj", "CoreLedger/"]
RUN dotnet restore "CoreLedger/CoreLedger.fsproj"
COPY . .
WORKDIR "/src/CoreLedger"
RUN dotnet build "CoreLedger.fsproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CoreLedger.fsproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CoreLedger.dll"]
