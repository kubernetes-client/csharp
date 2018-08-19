# Execute Unit tests
cd tests/KubernetesClient.Tests
dotnet restore
dotnet test --no-restore --no-build
if [[ $? != 0 ]]; then
    exit 1
fi

cd ../..