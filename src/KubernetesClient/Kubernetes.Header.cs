using System.Net.Http.Headers;
using k8s.Models;

namespace k8s
{
    public partial class Kubernetes
    {
        public virtual MediaTypeHeaderValue GetHeader(object body)
        {
            if (body == null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            if (body is V1Patch patch)
            {
                return GetHeader(patch);
            }

            return MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
        }


        public virtual MediaTypeHeaderValue GetHeader(V1Patch body)
        {
            if (body == null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            switch (body.Type)
            {
                case V1Patch.PatchType.JsonPatch:
                    return MediaTypeHeaderValue.Parse("application/json-patch+json; charset=utf-8");
                case V1Patch.PatchType.MergePatch:
                    return MediaTypeHeaderValue.Parse("application/merge-patch+json; charset=utf-8");
                case V1Patch.PatchType.StrategicMergePatch:
                    return MediaTypeHeaderValue.Parse("application/strategic-merge-patch+json; charset=utf-8");
                case V1Patch.PatchType.ApplyPatch:
                    return MediaTypeHeaderValue.Parse("application/apply-patch+yaml; charset=utf-8");
                default:
                    throw new ArgumentOutOfRangeException(nameof(body.Type), "");
            }
        }
    }
}
