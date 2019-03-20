#!/bin/sh
echo 'Installing .NET Core...'

wget https://download.visualstudio.microsoft.com/download/pr/4850aa8f-44a9-4c4a-9961-f18aa4d90ceb/07d790444f3ba6b412a76d6f1aced338/dotnet-sdk-2.2.105-osx-x64.pkg -O ~/dotnet-sdk-2.2.105-osx-x64.pkg

sudo installer -pkg ~/dotnet-sdk-2.2.105-osx-x64.pkg -target /

# https://github.com/dotnet/cli/issues/2544
ln -s /usr/local/share/dotnet/dotnet /usr/local/bin/
