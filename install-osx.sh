#!/bin/sh

echo 'Installing Brew...'
ruby -e "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/master/install)" < /dev/null 2> /dev/null

echo 'Installing .NET Core...'

wget https://download.microsoft.com/download/8/8/5/88544F33-836A-49A5-8B67-451C24709A8F/dotnet-sdk-2.1.300-osx-x64.pkg -O ~/dotnet-sdk-2.1.300-osx-x64.pkg
sudo installer -pkg ~/dotnet-sdk-2.1.300-osx-x64.pkg -target /

# https://github.com/dotnet/cli/issues/2544
ln -s /usr/local/share/dotnet/dotnet /usr/local/bin/

echo 'Installing code coverage tools'
brew install jq
wget -O ~/codacy-coverage-reporter-assembly-latest.jar $(curl https://api.github.com/repos/codacy/codacy-coverage-reporter/releases/latest | jq -r .assets[0].browser_download_url)
