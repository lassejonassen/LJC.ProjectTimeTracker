# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["src/ProjectTimeTracker.WebAPI/ProjectTimeTracker.WebAPI.csproj", "src/ProjectTimeTracker.WebAPI/"]
COPY ["src/ProjectTimeTracker.Application/ProjectTimeTracker.Application.csproj", "src/ProjectTimeTracker.Application/"]
COPY ["src/ProjectTimeTracker.SharedKernel/ProjectTimeTracker.SharedKernel.csproj", "src/ProjectTimeTracker.SharedKernel/"]
COPY ["src/ProjectTimeTracker.Domain/ProjectTimeTracker.Domain.csproj", "src/ProjectTimeTracker.Domain/"]
COPY ["src/ProjectTimeTracker.Infrastructure/ProjectTimeTracker.Infrastructure.csproj", "src/ProjectTimeTracker.Infrastructure/"]
COPY ["src/ProjectTimeTracker.WebAPI.Contracts/ProjectTimeTracker.WebAPI.Contracts.csproj", "src/ProjectTimeTracker.WebAPI.Contracts/"]
RUN dotnet restore "./src/ProjectTimeTracker.WebAPI/ProjectTimeTracker.WebAPI.csproj"
COPY . .
WORKDIR "/src/src/ProjectTimeTracker.WebAPI"
RUN dotnet build "./ProjectTimeTracker.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ProjectTimeTracker.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProjectTimeTracker.WebAPI.dll"]