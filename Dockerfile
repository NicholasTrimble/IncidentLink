# ==========================================
# STAGE 1: Build the application in the cloud
# ==========================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env
WORKDIR /app

# 1. Copy the project file first to restore dependencies
COPY *.csproj ./
RUN dotnet restore *.csproj

# 2. Copy the rest of your source code
COPY . .

# === SAFE TAILWIND INJECTION FOR RENDER ===
# Install Node.js inside the build container so it can process Tailwind utilities
RUN apt-get update && apt-get install -y curl
RUN curl -sL https://deb.nodesource.com/setup_20.x | bash -
RUN apt-get install -y nodejs

# Run the Tailwind compiler to generate the production output.css
RUN npx tailwindcss -i ./wwwroot/css/input.css -o ./wwwroot/css/output.css --minify
# ==========================================

# 3. Publish a compiled, optimized release of the app
RUN dotnet publish -c Release -o out

# ==========================================
# STAGE 2: Create the lightweight runtime container
# ==========================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Copy the compiled output from Stage 1 into this clean container
# (This automatically grabs the fresh output.css generated above)
COPY --from=build-env /app/out .

# Tell .NET to listen on the specific port Render uses
ENV ASPNETCORE_URLS=http://+:10000

# Start the application
ENTRYPOINT ["dotnet", "IncidentLink.dll"]