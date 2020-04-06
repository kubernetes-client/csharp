#!/bin/sh
echo 'Installing .NET Core...'

wget https://download.microsoft.com/download/3/a/3/3a3bda26-560d-4d8e-922e-6f6bc4553a84/dotnet-runtime-2.0.9-osx-x64.pkg -O ~/dotnet-runtime-2.0.9-osx-x64.pkg
wget https://download.visualstudio.microsoft.com/download/pr/9d3edcf8-2da1-42eb-a30f-54d629c8f13f/2e967304f8f3543c7329fd53d292d076/dotnet-runtime-2.1.17-osx-x64.pkg -O ~/dotnet-runtime-2.1.17-osx-x64.pkg
wget https://download.visualstudio.microsoft.com/download/pr/905598d0-17a3-4b42-bf13-c5a69d7aac87/853aff73920dcb013c09a74f05da7f6a/dotnet-sdk-3.1.201-osx-x64.pkg -O ~/dotnet-sdk-3.1.201-osx-x64.pkg

sudo installer -pkg ~/dotnet-runtime-2.0.9-osx-x64.pkg -target /
sudo installer -pkg ~/dotnet-runtime-2.1.17-osx-x64.pkg -target /
sudo installer -pkg ~/dotnet-sdk-3.1.201-osx-x64.pkg -target /

# https://github.com/dotnet/cli/issues/2544
ln -s /usr/local/share/dotnet/dotnet /usr/local/bin/
