# Execute Unit tests
cd tests/KubernetesClient.Tests
dotnet restore
dotnet test --no-restore --no-build /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=./coverage.xml
echo Uploading coverage...
java -jar ~/codacy-coverage-reporter-assembly-latest.jar report -l CSharp -r ./coverage.xml
if [[ $? != 0 ]]; then
    exit 1
fi

cd ../..