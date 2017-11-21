#!/usr/bin/env bash

# Exit on any error
set -e

# Ensure no compile errors in all projects
find . -name *.csproj -exec dotnet build {} \;

# Execute Unit tests
cd tests
dotnet restore
dotnet test
