#!/usr/bin/env bash

# Exit on any error
set -e

# Ensure no compile errors in all projects
find . -name *.csproj -exec dotnet build {} \;

# Execute Unit tests
cd tests/KubernetesClient.Tests
dotnet restore
dotnet test --no-restore --no-build /p:CollectCoverage=true /p:CoverletOutputFormat=lcov /p:CoverletOutput=./lcov.info
if [[ $? != 0 ]]; then
    exit 1
fi

