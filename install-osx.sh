#!/bin/sh
echo 'Installing .NET Core...'

wget https://dotnetcli.blob.core.windows.net/dotnet/Sdk/2.1.300-rc1-008673/dotnet-sdk-2.1.300-rc1-008673-osx-x64.pkg -O ~/dotnet-sdk-2.1.300-rc1-008673-osx-x64.pkg
sudo installer -pkg ~/dotnet-sdk-2.1.300-rc1-008673-osx-x64.pkg -target /

# https://github.com/dotnet/cli/issues/2544
ln -s /usr/local/share/dotnet/dotnet /usr/local/bin/
