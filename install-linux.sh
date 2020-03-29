#!/bin/sh
echo 'Installing .NET Core...'

curl https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > microsoft.gpg
sudo mv microsoft.gpg /etc/apt/trusted.gpg.d/microsoft.gpg
sudo sh -c 'echo "deb [arch=amd64] https://packages.microsoft.com/repos/microsoft-ubuntu-xenial-prod xenial main" > /etc/apt/sources.list.d/dotnetdev.list'
sudo apt-get -qq update
sudo apt-get install -y dotnet-runtime-2.0.9
sudo apt-get install -y dotnet-runtime-2.1
sudo apt-get install -y dotnet-sdk-3.1

echo 'Installing kubectl'
curl -Lo kubectl https://storage.googleapis.com/kubernetes-release/release/v1.13.4/bin/linux/amd64/kubectl
chmod +x kubectl
sudo mv kubectl /usr/local/bin/

echo 'Installing minikube'
curl -Lo minikube https://storage.googleapis.com/minikube/releases/v1.0.1/minikube-linux-amd64
chmod +x minikube
sudo mv minikube /usr/local/bin/

echo 'Creating the minikube cluster'
sudo minikube start --vm-driver=none --kubernetes-version=v1.13.4 --extra-config=apiserver.authorization-mode=RBAC
sudo chown -R $USER $HOME/.minikube
sudo chgrp -R $USER $HOME/.minikube
sudo chown -R $USER $HOME/.kube
sudo chgrp -R $USER $HOME/.kube

minikube update-context

echo 'Waiting for the cluster nodes to be ready'
JSONPATH='{range .items[*]}{@.metadata.name}:{range @.status.conditions[*]}{@.type}={@.status};{end}{end}'; \
  until kubectl get nodes -o jsonpath="$JSONPATH" 2>&1 | grep -q "Ready=True"; do sleep 1; done
