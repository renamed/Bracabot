
FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Bracabot2/Bracabot2.csproj", "Bracabot2/"]
RUN dotnet restore "Bracabot2/Bracabot2.csproj"
COPY . .
WORKDIR "/src/Bracabot2"
RUN dotnet build "Bracabot2.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Bracabot2.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bracabot2.exe"]