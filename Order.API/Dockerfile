FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AnalyticWebServiceWithClickHouse.Client/AnalyticWebServiceWithClickHouse.Client.csproj", "AnalyticWebServiceWithClickHouse.Client/"]
RUN dotnet restore "AnalyticWebServiceWithClickHouse.Client/AnalyticWebServiceWithClickHouse.Client.csproj"
COPY . .
WORKDIR "/src/AnalyticWebServiceWithClickHouse.Client"
RUN dotnet build "./AnalyticWebServiceWithClickHouse.Client.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AnalyticWebServiceWithClickHouse.Client.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AnalyticWebServiceWithClickHouse.Client.dll"]
