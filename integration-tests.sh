#!/bin/sh
cd examples

echo 'Creating a nginx pod in the default namespace'
kubectl create -f nginx.yml

echo 'Running the simple example'
cd simple
dotnet run

echo 'Running the exec example'
cd ../exec
dotnet run

echo 'Running the labels example'
cd ../labels
dotnet run

echo 'Running the namespace example'
cd ../namespace
dotnet run