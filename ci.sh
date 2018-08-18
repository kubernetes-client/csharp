#!/usr/bin/env bash

# Exit on any error
set -e

# Ensure no compile errors in all projects
find . -name *.csproj -exec dotnet build {} \;

# Create the NuGet package
cd src/KubernetesClient/
dotnet pack -c Release

if [[ $? != 0 ]]; then
    exit 1
fi

cd ../..