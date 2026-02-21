#!/bin/bash

set -e  # stop on first error

echo "🚀 Starting WatchHive deployment build..."

# =========================
# Variables
# =========================
PROJECT_NAME="WatchHive"
PROJECT_PATH="./WatchHive.csproj"
UI_PATH="./UI"
PUBLISH_DIR="./publish"
ZIP_NAME="watchhive-deploy.zip"

# =========================
# Clean old builds
# =========================
echo "🧹 Cleaning old publish directory..."
rm -rf $PUBLISH_DIR
rm -f $ZIP_NAME

# =========================
# Publish ASP.NET Core
# =========================
echo "⚙️ Publishing ASP.NET Core..."
dotnet publish $PROJECT_PATH -c Release -o $PUBLISH_DIR

# =========================
# Create ZIP
# =========================
echo " Creating deployment ZIP..."
cd $PUBLISH_DIR
zip -r ../$ZIP_NAME .
cd ..

# =========================
# Cleanup
# =========================
echo "Cleanup - deleting publish directory"
rm -rf publish/

echo "✅ Deployment package ready: $ZIP_NAME"