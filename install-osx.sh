#!/bin/sh
echo 'Installing .NET Core...'

wget https://download.microsoft.com/download/D/7/8/D788D3CD-44C4-487D-829B-413E914FB1C3/dotnet-sdk-2.1.300-preview1-008174-osx-x64.pkg -O ~/dotnet-sdk-2.1.300-preview1-008174-osx-x64.pkg
sudo installer -pkg ~/dotnet-sdk-2.1.300-preview1-008174-osx-x64.pkg -target /

# https://github.com/dotnet/cli/issues/2544
ln -s /usr/local/share/dotnet/dotnet /usr/local/bin/
