FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["EquiblesWebsockets/EquiblesWebsockets.csproj", "EquiblesWebsockets/"]
RUN dotnet restore "EquiblesWebsockets/EquiblesWebsockets.csproj"
COPY . .
WORKDIR "/src/EquiblesWebsockets"
RUN dotnet build "EquiblesWebsockets.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EquiblesWebsockets.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EquiblesWebsockets.dll"]
