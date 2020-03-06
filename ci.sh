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
# Save the test results to a file
# Collect code coverage of the KuberetsClient assembly, but exclude the
# auto-generated models from the coverage reports.
dotnet test \
    -l "trx;LogFileName=KubernetesClient.Tests.xunit.trx" \
    /p:CollectCoverage=true \
    /p:Include="[KubernetesClient]*" \
    /p:Exclude="[KubernetesClient]k8s.Models.*" \
    /p:Exclude="[KubernetesClient]k8s.Internal.*" \
    /p:CoverletOutputFormat="opencover" \
    /p:CoverletOutput="KubernetesClient.Tests.opencover.xml"

cd ..
echo Generating Code Coverage reports
export PATH="$PATH:$HOME/.dotnet/tools"
export DOTNET_ROOT=$(dirname $(realpath $(which dotnet))) # https://github.com/dotnet/cli/issues/9114#issuecomment-401670622
dotnet tool install --global dotnet-reportgenerator-globaltool --version 4.0.15
reportgenerator "-reports:**/*.opencover*.xml" "-targetdir:coveragereport" "-reporttypes:HTMLInline;Cobertura"

ls coveragereport
ls coveragereport/Cobertura.xml

cd ..
