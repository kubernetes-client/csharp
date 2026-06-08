#!/usr/bin/env bash
# Bring up a single node Kubernetes control plane inside the kindest/node rootfs
# that oci-to-wsl imported into WSL. kind normally runs this image as a Docker
# container and drives kubeadm from the host through `docker exec`; here WSL
# boots the image's systemd directly, so the equivalent bootstrap is performed
# in place.
set -euxo pipefail

# The kindest/node entrypoint (which WSL does not run) makes the root mount
# shared and enables IPv4 forwarding; reproduce the parts kubeadm and the
# kubelet rely on.
mount --make-rshared / 2>/dev/null || true
sysctl -w net.ipv4.ip_forward=1 || true

# containerd ships as a systemd unit in the node image.
systemctl enable --now containerd

# Wait for the container runtime to accept requests.
for _ in $(seq 1 60); do
  if ctr --namespace k8s.io version >/dev/null 2>&1; then
    break
  fi
  sleep 2
done

# Initialise the control plane. Preflight errors are ignored because the WSL
# environment intentionally differs from a vanilla node (swap on, kernel
# modules provided by the Windows-side kernel, and so on).
kubeadm init \
  --ignore-preflight-errors=all \
  --pod-network-cidr=10.244.0.0/16 \
  --apiserver-cert-extra-sans=127.0.0.1,localhost

export KUBECONFIG=/etc/kubernetes/admin.conf

# Single node cluster: allow workloads to schedule on the control-plane node.
kubectl taint nodes --all node-role.kubernetes.io/control-plane- 2>/dev/null || true

# kindest/node bundles the default CNI (kindnet) manifest.
kubectl apply -f /kind/manifests/default-cni.yaml

kubectl wait --for=condition=Ready nodes --all --timeout=180s
