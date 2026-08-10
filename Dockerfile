FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Back-End/Supermercado.API/Supermercado.API.csproj", "Back-End/Supermercado.API/"]
COPY ["Back-End/Supermercado.Data/Supermercado.Data.csproj", "Back-End/Supermercado.Data/"]
COPY ["Back-End/Supermercado.Domain/Supermercado.Domain.csproj", "Back-End/Supermercado.Domain/"]
COPY ["Back-End/Supermercado.Services/Supermercado.Services.csproj", "Back-End/Supermercado.Services/"]
RUN dotnet restore "Back-End/Supermercado.API/Supermercado.API.csproj"
COPY . .
WORKDIR "/src/Back-End/Supermercado.API"
RUN dotnet build "Supermercado.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Supermercado.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Supermercado.API.dll"]