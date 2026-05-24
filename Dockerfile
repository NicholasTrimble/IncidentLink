# ==========================================
# STAGE 1: Build the application in the cloud
# ==========================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env
WORKDIR /app

# 1. Copy the project file first to restore dependencies
COPY IncidentLink/*.csproj ./IncidentLink/
RUN dotnet restore IncidentLink/*.csproj

# 2. Copy the rest of your source code
COPY . .

# 3. Publish a compiled, optimized release of the app
RUN dotnet publish IncidentLink/IncidentLink.csproj -c Release -o out

# ==========================================
# STAGE 2: Create the lightweight runtime container
# ==========================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Copy the compiled output from Stage 1 into this clean container
COPY --from=build-env /app/out .

# Tell .NET to listen on the specific port Render uses
ENV ASPNETCORE_URLS=http://+:10000

# Start the application
ENTRYPOINT ["dotnet", "IncidentLink.dll"]