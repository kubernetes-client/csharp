#!/usr/bin/env bash

# Exit on any error
set -e

# Ensure no compile errors in all projects
dotnet restore
dotnet build --no-restore

# Execute Unit tests
cd tests
dotnet test --no-restore --no-build
if [[ $? != 0 ]]; then
    exit 1
fi
