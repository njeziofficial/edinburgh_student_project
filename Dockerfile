# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj and restore
COPY ["Edinburgh_Internation_Students/Edinburgh_Internation_Students.csproj", "Edinburgh_Internation_Students/"]
RUN dotnet restore "Edinburgh_Internation_Students/Edinburgh_Internation_Students.csproj"

# Copy everything else
COPY Edinburgh_Internation_Students/ Edinburgh_Internation_Students/

WORKDIR /src/Edinburgh_Internation_Students
RUN dotnet build "Edinburgh_Internation_Students.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Edinburgh_Internation_Students.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Edinburgh_Internation_Students.dll"]
