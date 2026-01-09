#!/bin/bash

# Docker Setup Test Script for Smart University
# This script validates the Docker deployment

set -e

echo "🧪 Smart University Docker Setup Test"
echo "===================================="

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Test results
TESTS_PASSED=0
TESTS_FAILED=0

# Function to run a test
run_test() {
    local test_name="$1"
    local test_command="$2"
    
    echo -n "Testing $test_name... "
    
    if eval "$test_command" >/dev/null 2>&1; then
        echo -e "${GREEN}✅ PASSED${NC}"
        ((TESTS_PASSED++))
    else
        echo -e "${RED}❌ FAILED${NC}"
        ((TESTS_FAILED++))
    fi
}

# Function to test HTTP endpoint
test_http() {
    local url="$1"
    local expected_status="$2"
    
    local status=$(curl -s -o /dev/null -w "%{http_code}" "$url" || echo "000")
    [ "$status" = "$expected_status" ]
}

# Function to test service health
test_service_health() {
    local service="$1"
    docker-compose ps "$service" | grep -q "Up"
}

echo "🔍 Running Docker setup tests..."
echo ""

# Test 1: Docker and Docker Compose availability
run_test "Docker availability" "command -v docker"
run_test "Docker Compose availability" "command -v docker-compose"

# Test 2: Docker daemon running
run_test "Docker daemon" "docker info"

# Test 3: Required files exist
run_test "Dockerfile exists" "[ -f Dockerfile ]"
run_test "docker-compose.yml exists" "[ -f docker-compose.yml ]"
run_test ".env file exists" "[ -f .env ]"

# Test 4: Docker Compose configuration
run_test "Docker Compose config validation" "docker-compose config"

# Test 5: Build application image
echo "🏗️  Building application image..."
if docker-compose build smart-university-app; then
    echo -e "${GREEN}✅ Application image built successfully${NC}"
    ((TESTS_PASSED++))
else
    echo -e "${RED}❌ Failed to build application image${NC}"
    ((TESTS_FAILED++))
fi

# Test 6: Start services
echo "🚀 Starting services..."
if docker-compose up -d; then
    echo -e "${GREEN}✅ Services started successfully${NC}"
    ((TESTS_PASSED++))
else
    echo -e "${RED}❌ Failed to start services${NC}"
    ((TESTS_FAILED++))
    exit 1
fi

# Wait for services to be ready
echo "⏳ Waiting for services to be ready..."
sleep 30

# Test 7: Service health checks
run_test "PostgreSQL service health" "test_service_health postgres"
run_test "RabbitMQ service health" "test_service_health rabbitmq"
run_test "Application service health" "test_service_health smart-university-app"

# Test 8: Database connectivity
run_test "Database connectivity" "docker-compose exec -T postgres pg_isready -U postgres -d smart_university"

# Test 9: RabbitMQ connectivity
run_test "RabbitMQ connectivity" "docker-compose exec -T rabbitmq rabbitmq-diagnostics ping"

# Test 10: Application endpoints
echo "🌐 Testing application endpoints..."
sleep 10  # Additional wait for application startup

run_test "Health endpoint" "test_http http://localhost:8080/health 200"
run_test "Swagger endpoint" "test_http http://localhost:8080/swagger 200"

# Test 11: Database migrations
echo "🗄️  Testing database migrations..."
if ./scripts/migrate-database.sh; then
    echo -e "${GREEN}✅ Database migrations completed${NC}"
    ((TESTS_PASSED++))
else
    echo -e "${RED}❌ Database migrations failed${NC}"
    ((TESTS_FAILED++))
fi

# Test 12: Application logs
run_test "Application logs available" "docker-compose logs smart-university-app | grep -q 'Application started'"

# Test 13: RabbitMQ Management UI
run_test "RabbitMQ Management UI" "test_http http://localhost:15672 200"

# Test 14: Database schemas
run_test "Database schemas created" "docker-compose exec -T postgres psql -U postgres -d smart_university -c '\dn' | grep -q identity"

# Test 15: Volume persistence
run_test "PostgreSQL volume mounted" "docker volume ls | grep -q postgres_data"
run_test "RabbitMQ volume mounted" "docker volume ls | grep -q rabbitmq_data"

# Test 16: Network connectivity
run_test "Internal network created" "docker network ls | grep -q smart-university-network"

# Test 17: Container resource usage
echo "📊 Checking container resource usage..."
docker stats --no-stream --format "table {{.Container}}\t{{.CPUPerc}}\t{{.MemUsage}}" | grep smart-university

# Test 18: Security checks
run_test "Non-root user in app container" "docker-compose exec -T smart-university-app whoami | grep -q appuser"

# Summary
echo ""
echo "📋 Test Summary"
echo "==============="
echo -e "Tests Passed: ${GREEN}$TESTS_PASSED${NC}"
echo -e "Tests Failed: ${RED}$TESTS_FAILED${NC}"

if [ $TESTS_FAILED -eq 0 ]; then
    echo -e "${GREEN}🎉 All tests passed! Docker setup is working correctly.${NC}"
    echo ""
    echo "🌐 Access Points:"
    echo "  Application: http://localhost:8080"
    echo "  Swagger UI:  http://localhost:8080/swagger"
    echo "  RabbitMQ UI: http://localhost:15672 (user: smartuni, pass: smartuni2024)"
    echo ""
    echo "📚 Useful Commands:"
    echo "  View logs:   docker-compose logs -f"
    echo "  Stop:        docker-compose down"
    echo "  Restart:     docker-compose restart"
    exit 0
else
    echo -e "${RED}❌ Some tests failed. Please check the output above.${NC}"
    echo ""
    echo "🔍 Troubleshooting:"
    echo "  Check logs:  docker-compose logs"
    echo "  Check status: docker-compose ps"
    echo "  Check config: docker-compose config"
    exit 1
fi