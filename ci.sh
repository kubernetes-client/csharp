#!/usr/bin/env bash

# Exit on any error
set -e

# Ensure no compile errors in all projects
find . -name *.csproj -exec dotnet build {} \;

# Create the NuGet package
cd src/KubernetesClient/
dotnet pack -c Release
cd ../..

# Execute Unit tests
cd tests/KubernetesClient.Tests
dotnet restore
dotnet test

cd ../..