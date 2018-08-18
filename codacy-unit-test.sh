# Execute Unit tests
cd tests/KubernetesClient.Tests
dotnet restore
dotnet test --no-restore --no-build /p:CollectCoverage=true /p:ExcludeByFile=\"*/generated/**/*.cs\" /p:CoverletOutputFormat=lcov /p:CoverletOutput=./coverage.xml
java -jar ~/codacy-coverage-reporter-assembly-latest.jar report -l csharp --forceLanguage -r ./coverage.xml
if [[ $? != 0 ]]; then
    exit 1
fi

cd ../..