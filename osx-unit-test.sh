# Execute Unit tests
cd tests/KubernetesClient.Tests
dotnet restore
dotnet test --no-restore --no-build /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=./coverage.xml
cd ../..
echo Uploading coverage...
java -jar ./tools/codacy-coverage-reporter.jar report -l CSharp -r ./tests/KubernetesClient.Tests/coverage.xml; 
if [[ $? != 0 ]]; then
    exit 1
fi

cd ../..