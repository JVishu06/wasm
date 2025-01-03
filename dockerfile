# Use the official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the project files and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the remaining source code and build the application
COPY . ./
RUN dotnet publish -c Release -o /out

# Use the official NGINX image to serve the app
FROM nginx:alpine
WORKDIR /usr/share/nginx/html

# Copy the published app files to the NGINX directory
COPY --from=build /out/wwwroot .

# Expose port 80
EXPOSE 80

# Start NGINX
CMD ["nginx", "-g", "daemon off;"]
