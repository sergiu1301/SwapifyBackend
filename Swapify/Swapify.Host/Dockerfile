# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Swapify.Host/Swapify.Host.csproj", "Swapify.Host/"]
COPY ["Swapify.API/Swapify.API.csproj", "Swapify.API/"]
COPY ["Swapify.Contracts/Swapify.Contracts.csproj", "Swapify.Contracts/"]
COPY ["Swapify.Infrastructure/Swapify.Infrastructure.csproj", "Swapify.Infrastructure/"]
COPY ["Swapify.Notifications/Swapify.Notifications.csproj", "Swapify.Notifications/"]
RUN dotnet restore "./Swapify.Host/Swapify.Host.csproj"
COPY . .
WORKDIR "/src/Swapify.Host"
RUN dotnet build "./Swapify.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Swapify.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Swapify.Host.dll"]