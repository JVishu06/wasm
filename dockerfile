# Stage 1: Build the WASM application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["wasm.csproj", "./"]
RUN dotnet restore "./wasm.csproj"

COPY . .
RUN dotnet build "./wasm.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./wasm.csproj" -c $BUILD_CONFIGURATION -o /app/dist /p:UseAppHost=false

# Stage 2: Serve using NGINX
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/dist .

# Copy a custom nginx.conf if needed (optional)
# COPY nginx.conf /etc/nginx/nginx.conf

# Expose default NGINX port
EXPOSE 80

ENTRYPOINT ["nginx", "-g", "daemon off;"]
