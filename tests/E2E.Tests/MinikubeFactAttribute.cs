using System;
using Xunit;

namespace k8s.E2E
{
    public sealed class MinikubeFactAttribute : FactAttribute
    {
        private static readonly bool HasEnv = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("K8S_E2E_MINIKUBE"));

        public MinikubeFactAttribute()
        {
            Skip = "K8S_E2_MINIKUBE not set";
            SkipType = typeof(MinikubeFactAttribute);
            SkipWhen = nameof(IsMinikubeDisabled);
        }

        public static bool IsMinikubeDisabled => !HasEnv;
    }
}
