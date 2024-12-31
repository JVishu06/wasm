# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy the project files and restore dependencies
COPY . ./
RUN dotnet restore

# Publish the app to the 'out' directory
RUN dotnet publish -c Release -o out

# Use the official NGINX image to serve the app
FROM nginx:alpine
WORKDIR /usr/share/nginx/html

# Copy the published app files to the NGINX directory
COPY --from=build-env /app/out/wwwroot .

# Expose port 80
EXPOSE 80

# Start NGINX
CMD ["nginx", "-g", "daemon off;"]