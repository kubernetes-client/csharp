# Execute Unit tests
cd tests/KubernetesClient.Tests
dotnet restore
dotnet test --no-restore --no-build /p:CollectCoverage=true /p:ExcludeByFile=\"*/generated/**/*.cs\" /p:CoverletOutputFormat=opencover /p:CoverletOutput=./opencover.xml
./../../tools/csmacnz.Coveralls --opencov -i ./opencover.xml --useRelativePaths --repoToken $COVERALLS_REPO_TOKEN --commitId $TRAVIS_COMMIT --commitBranch $TRAVIS_BRANCH --commitAuthor "$REPO_COMMIT_AUTHOR" --commitEmail "$REPO_COMMIT_AUTHOR_EMAIL" --commitMessage "$REPO_COMMIT_MESSAGE" --jobId $TRAVIS_JOB_ID  --serviceName "travis-ci"
if [[ $? != 0 ]]; then
    exit 1
fi

cd ../..