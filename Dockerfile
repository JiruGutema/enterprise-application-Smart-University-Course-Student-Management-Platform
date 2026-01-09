# Use the official .NET 10 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

# Set the working directory
WORKDIR /app

# Copy the project file and restore dependencies
COPY SmartUniversity.csproj ./
RUN dotnet restore

# Copy the entire source code
COPY . ./

# Build the application
RUN dotnet publish -c Release -o out

# Use the official .NET 10 runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

# Set the working directory
WORKDIR /app

# Copy the published application from the build stage
COPY --from=build /app/out .

# Create a non-root user for security
RUN useradd --create-home --shell /bin/bash appuser && chown -R appuser /app
USER appuser

# Expose the port the app runs on
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Set the entry point
ENTRYPOINT ["dotnet", "SmartUniversity.dll"]