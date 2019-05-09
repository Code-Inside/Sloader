FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["src/Sloader.Fiddle/Sloader.Fiddle.csproj", "src/Sloader.Fiddle/"]
RUN dotnet restore "src/Sloader.Fiddle/Sloader.Fiddle.csproj"
COPY . .
WORKDIR "/src/src/Sloader.Fiddle"
RUN dotnet build "Sloader.Fiddle.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Sloader.Fiddle.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Sloader.Fiddle.dll"]