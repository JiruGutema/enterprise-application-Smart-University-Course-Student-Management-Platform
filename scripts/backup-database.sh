#!/bin/bash

# Database Backup Script for Smart University
# This script creates backups of the PostgreSQL database

set -e

BACKUP_DIR="./backups"
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
BACKUP_FILE="smart_university_backup_${TIMESTAMP}.sql"

echo "💾 Smart University Database Backup"
echo "=================================="

# Create backup directory if it doesn't exist
mkdir -p "$BACKUP_DIR"

# Check if PostgreSQL container is running
if ! docker-compose ps | grep -q "smart-university-db"; then
    echo "❌ PostgreSQL container is not running. Please start with 'docker-compose up -d' first."
    exit 1
fi

echo "📦 Creating database backup..."
docker-compose exec -T postgres pg_dump -U postgres -d smart_university > "$BACKUP_DIR/$BACKUP_FILE"

# Compress the backup
echo "🗜️  Compressing backup..."
gzip "$BACKUP_DIR/$BACKUP_FILE"

echo "✅ Backup completed successfully!"
echo "📁 Backup saved to: $BACKUP_DIR/${BACKUP_FILE}.gz"
echo "📊 Backup size: $(du -h "$BACKUP_DIR/${BACKUP_FILE}.gz" | cut -f1)"

# Clean up old backups (keep last 7 days)
echo "🧹 Cleaning up old backups (keeping last 7 days)..."
find "$BACKUP_DIR" -name "smart_university_backup_*.sql.gz" -mtime +7 -delete

echo "📋 Available backups:"
ls -lh "$BACKUP_DIR"/smart_university_backup_*.sql.gz 2>/dev/null || echo "No backups found"