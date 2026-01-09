# Smart University - Docker Deployment Guide

This guide provides comprehensive instructions for deploying the Smart University application using Docker and Docker Compose.

## 🏗️ Architecture Overview

The Smart University application is containerized with the following services:

- **Application**: .NET 10 web application with modular architecture
- **PostgreSQL**: Database with multiple schemas for bounded contexts
- **RabbitMQ**: Message broker for event-driven communication
- **Health Checks**: Built-in monitoring and readiness probes

## 📋 Prerequisites

- Docker Engine 20.10+
- Docker Compose 2.0+
- 4GB+ available RAM
- 10GB+ available disk space

## 🚀 Quick Start

### 1. Initial Setup

```bash
# Clone the repository
git clone <repository-url>
cd SmartUniversity

# Run the setup script
./scripts/setup-docker.sh
```

### 2. Configure Environment

Update the `.env` file with your configuration:

```bash
# Database Configuration
ConnectionStrings__Default="Host=postgres;Database=smart_university;Username=postgres;Password=smartuni2024"

# JWT Configuration
JWT__Secret="your-jwt-secret-key"
JWT__Issuer="SmartUniversity"
JWT__Audience="SmartUniversityUsers"

# SMTP Configuration
SMTP__User="your-email@gmail.com"
SMTP__Password="your-app-password"
SMTP__Host="smtp.gmail.com"
SMTP__Port="587"

# RabbitMQ Configuration
RabbitMQ__Host="rabbitmq"
RabbitMQ__Username="smartuni"
RabbitMQ__Password="smartuni2024"

# OpenAI Configuration
OpenAi__ApiKey="your-openai-api-key"
```

### 3. Start the Application

```bash
# Development environment
docker-compose up -d

# Production environment
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

### 4. Run Database Migrations

```bash
# Wait for containers to be ready, then run migrations
./scripts/migrate-database.sh
```

## 🔧 Configuration Options

### Environment-Specific Configurations

#### Development
- Uses `docker-compose.override.yml` automatically
- Exposes different ports to avoid conflicts
- Includes volume mounts for hot reload
- Uses Development environment settings

#### Production
- Uses `docker-compose.prod.yml`
- Implements Docker secrets for sensitive data
- Includes resource limits and health checks
- Optimized for security and performance

### Service Configuration

#### PostgreSQL Database
```yaml
# Default configuration
POSTGRES_DB: smart_university
POSTGRES_USER: postgres
POSTGRES_PASSWORD: smartuni2024

# Schemas created automatically:
# - identity, courses, enrollment
# - assessments, notifications, ai, content
```

#### RabbitMQ Message Broker
```yaml
# Default configuration
RABBITMQ_DEFAULT_USER: smartuni
RABBITMQ_DEFAULT_PASS: smartuni2024

# Management UI: http://localhost:15672
# AMQP Port: 5672
```

#### Application Settings
```yaml
# Health check endpoints
GET /health      # Basic health status
GET /health/ready # Readiness probe
GET /health/live  # Liveness probe

# Application URL: http://localhost:8080
# Swagger UI: http://localhost:8080/swagger
```

## 📊 Monitoring and Health Checks

### Built-in Health Endpoints

- **`/health`**: Overall application health
- **`/health/ready`**: Readiness for traffic (includes DB connectivity)
- **`/health/live`**: Liveness probe (basic application status)

### Container Health Checks

All services include health checks:
- **PostgreSQL**: `pg_isready` command
- **RabbitMQ**: `rabbitmq-diagnostics ping`
- **Application**: HTTP health endpoint

### Monitoring Commands

```bash
# Check service status
docker-compose ps

# View logs
docker-compose logs -f [service-name]

# Check health status
curl http://localhost:8080/health

# Monitor resource usage
docker stats
```

## 🗄️ Database Management

### Migrations

```bash
# Run all migrations
./scripts/migrate-database.sh

# Run specific context migration
docker-compose exec smart-university-app dotnet ef database update --context UserDbContext
```

### Backup and Restore

```bash
# Create backup
./scripts/backup-database.sh

# Restore from backup
docker-compose exec -T postgres psql -U postgres -d smart_university < backup.sql
```

### Database Access

```bash
# Connect to PostgreSQL
docker-compose exec postgres psql -U postgres -d smart_university

# View schemas and tables
\dt *.*

# Check specific schema
\dt identity.*
```

## 🔐 Security Considerations

### Production Secrets

For production deployments, use Docker secrets:

```bash
# Create secret files
echo "your-secure-password" | docker secret create db_password -
echo "your-jwt-secret" | docker secret create jwt_secret -

# Use in docker-compose.prod.yml
secrets:
  - db_password
  - jwt_secret
```

### Network Security

- Services communicate through internal Docker network
- Only necessary ports are exposed externally
- Database and RabbitMQ are not directly accessible in production

### Environment Variables

- Sensitive data should use secrets in production
- Environment variables are logged - avoid sensitive data
- Use `.env` files for development only

## 🚀 Deployment Strategies

### Development Deployment

```bash
# Start with hot reload
docker-compose up -d

# View logs
docker-compose logs -f smart-university-app

# Rebuild after code changes
docker-compose build smart-university-app
docker-compose up -d smart-university-app
```

### Production Deployment

```bash
# Deploy with production configuration
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d

# Scale application (if needed)
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d --scale smart-university-app=3

# Rolling update
docker-compose -f docker-compose.yml -f docker-compose.prod.yml build smart-university-app
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d --no-deps smart-university-app
```

### CI/CD Integration

```yaml
# Example GitHub Actions workflow
name: Deploy Smart University
on:
  push:
    branches: [main]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Deploy to production
        run: |
          docker-compose -f docker-compose.yml -f docker-compose.prod.yml pull
          docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

## 🛠️ Troubleshooting

### Common Issues

#### Application Won't Start
```bash
# Check logs
docker-compose logs smart-university-app

# Check database connectivity
docker-compose exec smart-university-app dotnet ef database update --context UserDbContext --dry-run
```

#### Database Connection Issues
```bash
# Check PostgreSQL status
docker-compose exec postgres pg_isready -U postgres

# Verify connection string
docker-compose exec smart-university-app env | grep ConnectionStrings
```

#### RabbitMQ Connection Issues
```bash
# Check RabbitMQ status
docker-compose exec rabbitmq rabbitmq-diagnostics ping

# Check management UI
curl http://localhost:15672
```

### Performance Tuning

#### Database Optimization
```sql
-- Check database performance
SELECT schemaname, tablename, attname, n_distinct, correlation 
FROM pg_stats 
WHERE schemaname IN ('identity', 'courses', 'enrollment');
```

#### Application Optimization
```bash
# Monitor memory usage
docker stats smart-university-app

# Check garbage collection
docker-compose exec smart-university-app dotnet-counters monitor --process-id 1
```

## 📚 Additional Resources

### Useful Commands

```bash
# Complete cleanup
docker-compose down -v --remove-orphans
docker system prune -a

# Export/Import volumes
docker run --rm -v smart_university_postgres_data:/data -v $(pwd):/backup alpine tar czf /backup/postgres_backup.tar.gz -C /data .

# Update images
docker-compose pull
docker-compose up -d
```

### File Structure

```
SmartUniversity/
├── docker-compose.yml           # Main compose file
├── docker-compose.override.yml  # Development overrides
├── docker-compose.prod.yml      # Production configuration
├── Dockerfile                   # Application container
├── .dockerignore               # Docker ignore rules
├── init-db.sql                 # Database initialization
├── scripts/
│   ├── setup-docker.sh         # Initial setup
│   ├── migrate-database.sh     # Migration runner
│   └── backup-database.sh      # Backup utility
├── secrets/                    # Production secrets
└── logs/                       # Application logs
```

### Support

For issues and questions:
1. Check the troubleshooting section above
2. Review application logs: `docker-compose logs -f`
3. Check health endpoints: `curl http://localhost:8080/health`
4. Verify configuration: `docker-compose config`

---

**Note**: This Docker setup is designed for both development and production use. Always review and customize the configuration for your specific deployment requirements.