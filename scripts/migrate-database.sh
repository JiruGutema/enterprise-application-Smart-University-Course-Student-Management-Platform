#!/bin/bash

# Database Migration Script for Smart University
# This script runs Entity Framework migrations in the Docker environment

set -e

echo "🗄️  Smart University Database Migration"
echo "======================================"

# Check if Docker containers are running
if ! docker compose ps | grep -q "smart-university-app"; then
    echo "❌ Application container is not running. Please start with 'docker compose up -d' first."
    exit 1
fi

echo "📊 Running database migrations..."

# Run migrations for each DbContext
echo "🔄 Running Identity migrations..."
docker compose exec smart-university-app dotnet ef database update --context UserDbContext

echo "🔄 Running Course migrations..."
docker compose exec smart-university-app dotnet ef database update --context CourseDbContext

echo "🔄 Running Enrollment migrations..."
docker compose exec smart-university-app dotnet ef database update --context EnrollmentDbContext

echo "🔄 Running Grading migrations..."
docker compose exec smart-university-app dotnet ef database update --context GradingDbContext

echo "🔄 Running Notification migrations..."
docker compose exec smart-university-app dotnet ef database update --context NotificationDbContext

echo "🔄 Running AI migrations..."
docker compose exec smart-university-app dotnet ef database update --context AIDbContext

echo "🔄 Running Content migrations..."
docker compose exec smart-university-app dotnet ef database update --context ContentDbContext

echo "✅ All migrations completed successfully!"
echo ""
echo "📋 Database status:"
docker compose exec postgres psql -U postgres -d smart_university -c "\dt *.*" | head -20