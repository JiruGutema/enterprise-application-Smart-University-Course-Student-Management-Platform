#!/bin/bash

# Smart University Docker Setup Script
# This script helps set up the Docker environment for the Smart University application

set -e

echo "🎓 Smart University Docker Setup"
echo "================================"

# Function to check if a command exists
command_exists() {
    command -v "$1" >/dev/null 2>&1
}

# Check prerequisites
echo "📋 Checking prerequisites..."

if ! command_exists docker; then
    echo "❌ Docker is not installed. Please install Docker first."
    exit 1
fi

if ! docker compose version >/dev/null 2>&1; then
    echo "❌ Docker Compose is not available. Please install Docker Compose first."
    exit 1
fi

echo "✅ Docker and Docker Compose are installed"

# Create necessary directories
echo "📁 Creating necessary directories..."
mkdir -p secrets
mkdir -p logs
mkdir -p data/postgres
mkdir -p data/rabbitmq

# Create .env file if it doesn't exist
if [ ! -f .env ]; then
    echo "📝 Creating .env file from template..."
    cp .env.example .env
    echo "⚠️  Please update the .env file with your actual configuration values"
fi

# Create secrets directory for production
echo "🔐 Setting up secrets for production..."
if [ ! -f secrets/db_password.txt ]; then
    echo "smartuni2024" > secrets/db_password.txt
fi

if [ ! -f secrets/jwt_secret.txt ]; then
    echo "Kj82Jf9\$QmLxP7Z@W2#E8R!dVt4s0A1BAddisAbabaUniversity" > secrets/jwt_secret.txt
fi

if [ ! -f secrets/rabbitmq_user.txt ]; then
    echo "smartuni" > secrets/rabbitmq_user.txt
fi

if [ ! -f secrets/rabbitmq_password.txt ]; then
    echo "smartuni2024" > secrets/rabbitmq_password.txt
fi

if [ ! -f secrets/smtp_user.txt ]; then
    echo "your-email@gmail.com" > secrets/smtp_user.txt
fi

if [ ! -f secrets/smtp_password.txt ]; then
    echo "your-app-password" > secrets/smtp_password.txt
fi

if [ ! -f secrets/openai_api_key.txt ]; then
    echo "your-openai-api-key" > secrets/openai_api_key.txt
fi

# Set appropriate permissions for secrets
chmod 600 secrets/*

echo "✅ Setup completed successfully!"
echo ""
echo "🚀 Next steps:"
echo "1. Update the .env file with your actual configuration"
echo "2. Update the secrets files in the secrets/ directory"
echo "3. Run 'docker compose up -d' to start the application"
echo ""
echo "📚 Available commands:"
echo "  Development: docker compose up -d"
echo "  Production:  docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d"
echo "  Logs:        docker compose logs -f"
echo "  Stop:        docker compose down"
echo ""
echo "🌐 Access points:"
echo "  Application: http://localhost:8080"
echo "  RabbitMQ UI: http://localhost:15672 (user: smartuni, pass: smartuni2024)"
echo "  PostgreSQL:  localhost:5432 (user: postgres, pass: smartuni2024)"