#!/bin/sh
echo 'Installing .NET Core...'

wget https://download.visualstudio.microsoft.com/download/pr/3fb2ae01-c8c5-4d0a-9102-31c8c3386bc5/94b144257db9c52405d7f7e03adc31a9/dotnet-sdk-2.2.104-osx-gs-x64.pkg -O ~/dotnet-sdk-2.2.104-osx-gs-x64.pkg

sudo installer -pkg ~/dotnet-sdk-2.2.104-osx-gs-x64.pkg -target /

# https://github.com/dotnet/cli/issues/2544
ln -s /usr/local/share/dotnet/dotnet /usr/local/bin/
