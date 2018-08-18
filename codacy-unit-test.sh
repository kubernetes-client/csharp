# Execute Unit tests
cd tests/KubernetesClient.Tests
dotnet restore
dotnet test --no-restore --no-build /p:CollectCoverage=true /p:ExcludeByFile=\"*/generated/**/*.cs\" /p:CoverletOutputFormat=lcov /p:CoverletOutput=./coverage.xml
if [[ $? != 0 ]]; then
    exit 1
fi

cd ../..