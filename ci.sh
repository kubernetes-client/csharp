#!/usr/bin/env bash

# Exit on any error
set -e

# Execute Unit tests
cd tests
dotnet restore
dotnet xunit