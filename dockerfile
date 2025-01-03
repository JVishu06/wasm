# Use the official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the project files and restore dependencies
COPY *.csproj . 
RUN dotnet restore

# Copy the remaining source code and build the application
COPY . . 
RUN dotnet publish -c Release -o /out

# Use the official ASP.NET Core runtime image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the published files from the build stage
COPY --from=build /out .

# Expose the port for the app
EXPOSE 80

# Serve the Blazor WebAssembly app using ASP.NET Core server (Kestrel)
ENTRYPOINT ["dotnet", "wasm.dll"]
