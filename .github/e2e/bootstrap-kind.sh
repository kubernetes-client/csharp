#!/usr/bin/env bash
# Bring up a single node Kubernetes control plane inside the kindest/node rootfs
# that oci-to-wsl imported into WSL. kind normally runs this image as a Docker
# container and drives kubeadm from the host through `docker exec`; here WSL
# boots the image's systemd directly, so the equivalent bootstrap is performed
# in place.
set -euxo pipefail

# --------------------------------------------------------------------------
# Reproduce the setup that kind's container entrypoint (/usr/local/bin/entrypoint)
# performs when running kindest/node inside Docker. WSL boots systemd directly
# from the image's rootfs, so these steps must be done explicitly.
# --------------------------------------------------------------------------

# 1. Mount propagation — kubeadm and kubelet need shared mounts.
mount --make-rshared / 2>/dev/null || true

# 2. /dev/kmsg — kubelet reads kernel messages from this device and exits
#    immediately if it is missing. WSL2 does not create it by default.
if [ ! -e /dev/kmsg ]; then
  ln -sf /dev/console /dev/kmsg
fi

# 3. Kernel parameters required by kube-proxy and networking.
sysctl -w net.ipv4.ip_forward=1 || true
sysctl -w net.ipv4.conf.all.forwarding=1 || true
sysctl -w net.ipv6.conf.all.forwarding=1 2>/dev/null || true

# 4. Ensure /run is a tmpfs (systemd usually handles this, but be safe).
if ! mountpoint -q /run; then
  mount -t tmpfs tmpfs /run
  mkdir -p /run/lock
fi

# 5. Ensure a machine-id exists (kubelet uses it as node identity).
if [ ! -s /etc/machine-id ]; then
  systemd-machine-id-setup 2>/dev/null || uuidgen | tr -d '-' > /etc/machine-id
fi

# 6. containerd configuration — use cgroupfs driver. WSL2's systemd has limited
#    cgroup delegation, so the cgroupfs driver is more reliable here.
mkdir -p /etc/containerd
containerd config default \
  | sed 's/SystemdCgroup = true/SystemdCgroup = false/' \
  > /etc/containerd/config.toml

# 7. Start containerd.
systemctl enable --now containerd
systemctl restart containerd

# Wait for the container runtime to accept requests.
containerd_ready=0
for _ in {1..60}; do
  if ctr --namespace k8s.io version >/dev/null 2>&1; then
    containerd_ready=1
    break
  fi
  sleep 2
done
if [ "$containerd_ready" -ne 1 ]; then
  echo "containerd did not become ready after 120s" >&2
  systemctl status containerd --no-pager || true
  journalctl -xeu containerd --no-pager -n 80 || true
  exit 1
fi

# 8. The kindest/node image ships a kubelet drop-in (11-kind.conf) whose
#    ExecStartPre runs /kind/bin/create-kubelet-cgroup-v2.sh. That script
#    expects Docker-managed cgroup namespaces and fails in bare WSL2.
#    Patch it out so the kubelet service can start.
if [ -f /etc/systemd/system/kubelet.service.d/11-kind.conf ]; then
  sed -i '/create-kubelet-cgroup-v2/d' /etc/systemd/system/kubelet.service.d/11-kind.conf
  systemctl daemon-reload
fi
systemctl stop kubelet 2>/dev/null || true

# Initialize the control plane using a kubeadm config that forces cgroupfs and
# disables swap checking.
set +e
kubeadm init \
  --ignore-preflight-errors=all \
  --config /dev/stdin <<'KUBEADM_CONFIG'
apiVersion: kubeadm.k8s.io/v1beta4
kind: InitConfiguration
nodeRegistration:
  criSocket: unix:///run/containerd/containerd.sock
  ignorePreflightErrors:
    - all
---
apiVersion: kubeadm.k8s.io/v1beta4
kind: ClusterConfiguration
networking:
  podSubnet: 10.244.0.0/16
apiServer:
  certSANs:
    - "127.0.0.1"
    - "localhost"
---
apiVersion: kubelet.config.k8s.io/v1beta1
kind: KubeletConfiguration
cgroupDriver: cgroupfs
failSwapOn: false
KUBEADM_CONFIG
rc=$?
set -e

if [ $rc -ne 0 ]; then
  echo "=== kubeadm init failed (rc=$rc) — collecting diagnostics ===" >&2
  systemctl status kubelet --no-pager 2>&1 || true
  journalctl -xeu kubelet --no-pager -n 80 2>&1 || true
  echo "=== cgroup info ===" >&2
  mount | grep cgroup || true
  cat /proc/self/cgroup 2>/dev/null || true
  ls /sys/fs/cgroup/ 2>/dev/null || true
  exit 1
fi

export KUBECONFIG=/etc/kubernetes/admin.conf

# Single node cluster: allow workloads to schedule on the control-plane node.
kubectl taint nodes --all node-role.kubernetes.io/control-plane- 2>/dev/null || true

# The bundled kindnet manifest (/kind/manifests/default-cni.yaml) contains Go
# template placeholders (e.g. {{ .PodSubnet }}) that kind normally renders at
# cluster creation time. Render the template and apply; if that fails, install
# a standalone CNI config for host-local networking on this single node.
if ! sed -e 's/{{ \.PodSubnet }}/10.244.0.0\/16/g' \
         -e 's/{{ \.PodSubnet}}/10.244.0.0\/16/g' \
         -e 's/{{\.PodSubnet}}/10.244.0.0\/16/g' \
         -e '/^{{/d' -e '/^}}/d' \
         /kind/manifests/default-cni.yaml | kubectl apply -f - 2>&1; then
  echo "kindnet manifest apply failed; installing host-local CNI config directly"
  mkdir -p /etc/cni/net.d
  cat > /etc/cni/net.d/10-kindnet.conflist <<'CNI'
{
  "cniVersion": "0.4.0",
  "name": "kindnet",
  "plugins": [
    {
      "type": "ptp",
      "ipMasq": false,
      "ipam": { "type": "host-local", "dataDir": "/run/cni-ipam-state", "routes": [{"dst": "0.0.0.0/0"}], "ranges": [[{"subnet": "10.244.0.0/24"}]] }
    },
    { "type": "portmap", "capabilities": {"portMappings": true} }
  ]
}
CNI
fi

# Assign podCIDR to the node so kubelet can allocate pod IPs.
NODE=$(kubectl get nodes -o jsonpath='{.items[0].metadata.name}')
kubectl patch node "$NODE" -p '{"spec":{"podCIDR":"10.244.0.0/24","podCIDRs":["10.244.0.0/24"]}}' 2>/dev/null || true

kubectl wait --for=condition=Ready nodes --all --timeout=300s
