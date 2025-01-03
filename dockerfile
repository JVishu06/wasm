# Stage 1: Build the WASM application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the WASM project file and restore dependencies
COPY ["wasm.csproj", "./"]
RUN dotnet restore "./wasm.csproj"

# Copy the entire project and build it
COPY . .
RUN dotnet build "./wasm.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the WASM app to a dist folder
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./wasm.csproj" -c $BUILD_CONFIGURATION -o /app/dist /p:UseAppHost=false

# Stage 2: Serve the WASM app using NGINX for production
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

# Copy the published artifacts from the publish stage
COPY --from=publish /app/dist .

# Expose the HTTP port
EXPOSE 80

# Use NGINX as the entry point
ENTRYPOINT ["nginx", "-g", "daemon off;"]
