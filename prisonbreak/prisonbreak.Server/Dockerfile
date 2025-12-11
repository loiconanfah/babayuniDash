FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["prisonbreak.Server.csproj", "./"]
RUN dotnet restore "prisonbreak.Server.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "prisonbreak.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "prisonbreak.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "prisonbreak.Server.dll"]

