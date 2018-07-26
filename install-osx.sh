#!/bin/sh
echo 'Installing .NET Core...'

wget https://download.microsoft.com/download/8/8/5/88544F33-836A-49A5-8B67-451C24709A8F/dotnet-sdk-2.1.300-osx-x64.pkg -O ~/dotnet-sdk-2.1.300-osx-x64.pkg
sudo installer -pkg ~/dotnet-sdk-2.1.300-osx-x64.pkg -target /

# https://github.com/dotnet/cli/issues/2544
ln -s /usr/local/share/dotnet/dotnet /usr/local/bin/
