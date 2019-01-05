#!/bin/sh
echo 'Installing .NET Core...'

wget https://download.visualstudio.microsoft.com/download/pr/38102737-cb48-46c2-8f52-fb7102b50ae7/d81958d71c3c2679796e1ecfbd9cc903/dotnet-sdk-2.1.403-osx-x64.pkg -O ~/dotnet-sdk-2.1.403-osx-x64.pkg

sudo installer -pkg ~/dotnet-sdk-2.1.403-osx-x64.pkg -target /

# https://github.com/dotnet/cli/issues/2544
ln -s /usr/local/share/dotnet/dotnet /usr/local/bin/

echo 'Installing code coverage tools'

brew install jq
wget -O ./tools/codacy-coverage-reporter.jar $(curl https://api.github.com/repos/codacy/codacy-coverage-reporter/releases/latest | jq -r .assets[0].browser_download_url)
