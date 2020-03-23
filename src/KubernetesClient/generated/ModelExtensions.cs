namespace k8s.Models
{
    [KubernetesEntity(Group="admissionregistration.k8s.io", Kind="MutatingWebhookConfiguration", ApiVersion="v1", PluralName="mutatingwebhookconfigurations")]
    public partial class V1MutatingWebhookConfiguration : IKubernetesObject, IMetadata<V1ObjectMeta>
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "MutatingWebhookConfiguration";
        public const string KubeGroup = "admissionregistration.k8s.io";
    }

    [KubernetesEntity(Group="admissionregistration.k8s.io", Kind="MutatingWebhookConfigurationList", ApiVersion="v1", PluralName="mutatingwebhookconfigurations")]
    public partial class V1MutatingWebhookConfigurationList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1MutatingWebhookConfiguration>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "MutatingWebhookConfigurationList";
        public const string KubeGroup = "admissionregistration.k8s.io";
    }

    [KubernetesEntity(Group="admissionregistration.k8s.io", Kind="ValidatingWebhookConfiguration", ApiVersion="v1", PluralName="validatingwebhookconfigurations")]
    public partial class V1ValidatingWebhookConfiguration : IKubernetesObject, IMetadata<V1ObjectMeta>
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "ValidatingWebhookConfiguration";
        public const string KubeGroup = "admissionregistration.k8s.io";
    }

    [KubernetesEntity(Group="admissionregistration.k8s.io", Kind="ValidatingWebhookConfigurationList", ApiVersion="v1", PluralName="validatingwebhookconfigurations")]
    public partial class V1ValidatingWebhookConfigurationList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1ValidatingWebhookConfiguration>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "ValidatingWebhookConfigurationList";
        public const string KubeGroup = "admissionregistration.k8s.io";
    }

    [KubernetesEntity(Group="admissionregistration.k8s.io", Kind="MutatingWebhookConfiguration", ApiVersion="v1beta1", PluralName="mutatingwebhookconfigurations")]
    public partial class V1beta1MutatingWebhookConfiguration : IKubernetesObject, IMetadata<V1ObjectMeta>
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "MutatingWebhookConfiguration";
        public const string KubeGroup = "admissionregistration.k8s.io";
    }

    [KubernetesEntity(Group="admissionregistration.k8s.io", Kind="MutatingWebhookConfigurationList", ApiVersion="v1beta1", PluralName="mutatingwebhookconfigurations")]
    public partial class V1beta1MutatingWebhookConfigurationList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1beta1MutatingWebhookConfiguration>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "MutatingWebhookConfigurationList";
        public const string KubeGroup = "admissionregistration.k8s.io";
    }

    [KubernetesEntity(Group="admissionregistration.k8s.io", Kind="ValidatingWebhookConfiguration", ApiVersion="v1beta1", PluralName="validatingwebhookconfigurations")]
    public partial class V1beta1ValidatingWebhookConfiguration : IKubernetesObject, IMetadata<V1ObjectMeta>
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "ValidatingWebhookConfiguration";
        public const string KubeGroup = "admissionregistration.k8s.io";
    }

    [KubernetesEntity(Group="admissionregistration.k8s.io", Kind="ValidatingWebhookConfigurationList", ApiVersion="v1beta1", PluralName="validatingwebhookconfigurations")]
    public partial class V1beta1ValidatingWebhookConfigurationList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1beta1ValidatingWebhookConfiguration>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "ValidatingWebhookConfigurationList";
        public const string KubeGroup = "admissionregistration.k8s.io";
    }

    [KubernetesEntity(Group="apps", Kind="ControllerRevision", ApiVersion="v1", PluralName="controllerrevisions")]
    public partial class V1ControllerRevision : IKubernetesObject, IMetadata<V1ObjectMeta>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "ControllerRevision";
        public const string KubeGroup = "apps";
    }

    [KubernetesEntity(Group="apps", Kind="ControllerRevisionList", ApiVersion="v1", PluralName="controllerrevisions")]
    public partial class V1ControllerRevisionList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1ControllerRevision>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "ControllerRevisionList";
        public const string KubeGroup = "apps";
    }

    [KubernetesEntity(Group="apps", Kind="DaemonSet", ApiVersion="v1", PluralName="daemonsets")]
    public partial class V1DaemonSet : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1DaemonSetSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "DaemonSet";
        public const string KubeGroup = "apps";
    }

    [KubernetesEntity(Group="apps", Kind="DaemonSetList", ApiVersion="v1", PluralName="daemonsets")]
    public partial class V1DaemonSetList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1DaemonSet>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "DaemonSetList";
        public const string KubeGroup = "apps";
    }

    [KubernetesEntity(Group="apps", Kind="Deployment", ApiVersion="v1", PluralName="deployments")]
    public partial class V1Deployment : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1DeploymentSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "Deployment";
        public const string KubeGroup = "apps";
    }

    [KubernetesEntity(Group="apps", Kind="DeploymentList", ApiVersion="v1", PluralName="deployments")]
    public partial class V1DeploymentList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1Deployment>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "DeploymentList";
        public const string KubeGroup = "apps";
    }

    [KubernetesEntity(Group="apps", Kind="ReplicaSet", ApiVersion="v1", PluralName="replicasets")]
    public partial class V1ReplicaSet : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1ReplicaSetSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "ReplicaSet";
        public const string KubeGroup = "apps";
    }

    [KubernetesEntity(Group="apps", Kind="ReplicaSetList", ApiVersion="v1", PluralName="replicasets")]
    public partial class V1ReplicaSetList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1ReplicaSet>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "ReplicaSetList";
        public const string KubeGroup = "apps";
    }

    [KubernetesEntity(Group="apps", Kind="StatefulSet", ApiVersion="v1", PluralName="statefulsets")]
    public partial class V1StatefulSet : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1StatefulSetSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "StatefulSet";
        public const string KubeGroup = "apps";
    }

    [KubernetesEntity(Group="apps", Kind="StatefulSetList", ApiVersion="v1", PluralName="statefulsets")]
    public partial class V1StatefulSetList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1StatefulSet>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "StatefulSetList";
        public const string KubeGroup = "apps";
    }

    [KubernetesEntity(Group="apps", Kind="ControllerRevision", ApiVersion="v1beta1", PluralName="controllerrevisions")]
    public partial class V1beta1ControllerRevision : IKubernetesObject, IMetadata<V1ObjectMeta>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "ControllerRevision";
        public const string KubeGroup = "apps";
    }

    [KubernetesEntity(Group="apps", Kind="ControllerRevisionList", ApiVersion="v1beta1", PluralName="controllerrevisions")]
    public partial class V1beta1ControllerRevisionList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1beta1ControllerRevision>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "ControllerRevisionList";
        public const string KubeGroup = "apps";
    }

    [KubernetesEntity(Group="apps", Kind="Deployment", ApiVersion="v1beta1", PluralName="deployments")]
    public partial class Appsv1beta1Deployment : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<Appsv1beta1DeploymentSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "Deployment";
        public const string KubeGroup = "apps";
    }

    [KubernetesEntity(Group="apps", Kind="DeploymentList", ApiVersion="v1beta1", PluralName="deployments")]
    public partial class Appsv1beta1DeploymentList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<Appsv1beta1Deployment>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "DeploymentList";
        public const string KubeGroup = "apps";
    }

    [KubernetesEntity(Group="apps", Kind="DeploymentRollback", ApiVersion="v1beta1", PluralName=null)]
    public partial class Appsv1beta1DeploymentRollback : IKubernetesObject, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "DeploymentRollback";
        public const string KubeGroup = "apps";
    }

    [KubernetesEntity(Group="apps", Kind="Scale", ApiVersion="v1beta1", PluralName=null)]
    public partial class Appsv1beta1Scale : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<Appsv1beta1ScaleSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "Scale";
        public const string KubeGroup = "apps";
    }

    [KubernetesEntity(Group="apps", Kind="StatefulSet", ApiVersion="v1beta1", PluralName="statefulsets")]
    public partial class V1beta1StatefulSet : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1beta1StatefulSetSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "StatefulSet";
        public const string KubeGroup = "apps";
    }

    [KubernetesEntity(Group="apps", Kind="StatefulSetList", ApiVersion="v1beta1", PluralName="statefulsets")]
    public partial class V1beta1StatefulSetList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1beta1StatefulSet>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "StatefulSetList";
        public const string KubeGroup = "apps";
    }

    [KubernetesEntity(Group="apps", Kind="ControllerRevision", ApiVersion="v1beta2", PluralName="controllerrevisions")]
    public partial class V1beta2ControllerRevision : IKubernetesObject, IMetadata<V1ObjectMeta>, IValidate
    {
        public const string KubeApiVersion = "v1beta2";
        public const string KubeKind = "ControllerRevision";
        public const string KubeGroup = "apps";
    }

    [KubernetesEntity(Group="apps", Kind="ControllerRevisionList", ApiVersion="v1beta2", PluralName="controllerrevisions")]
    public partial class V1beta2ControllerRevisionList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1beta2ControllerRevision>, IValidate
    {
        public const string KubeApiVersion = "v1beta2";
        public const string KubeKind = "ControllerRevisionList";
        public const string KubeGroup = "apps";
    }

    [KubernetesEntity(Group="apps", Kind="DaemonSet", ApiVersion="v1beta2", PluralName="daemonsets")]
    public partial class V1beta2DaemonSet : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1beta2DaemonSetSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta2";
        public const string KubeKind = "DaemonSet";
        public const string KubeGroup = "apps";
    }

    [KubernetesEntity(Group="apps", Kind="DaemonSetList", ApiVersion="v1beta2", PluralName="daemonsets")]
    public partial class V1beta2DaemonSetList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1beta2DaemonSet>, IValidate
    {
        public const string KubeApiVersion = "v1beta2";
        public const string KubeKind = "DaemonSetList";
        public const string KubeGroup = "apps";
    }

    [KubernetesEntity(Group="apps", Kind="Deployment", ApiVersion="v1beta2", PluralName="deployments")]
    public partial class V1beta2Deployment : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1beta2DeploymentSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta2";
        public const string KubeKind = "Deployment";
        public const string KubeGroup = "apps";
    }

    [KubernetesEntity(Group="apps", Kind="DeploymentList", ApiVersion="v1beta2", PluralName="deployments")]
    public partial class V1beta2DeploymentList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1beta2Deployment>, IValidate
    {
        public const string KubeApiVersion = "v1beta2";
        public const string KubeKind = "DeploymentList";
        public const string KubeGroup = "apps";
    }

    [KubernetesEntity(Group="apps", Kind="ReplicaSet", ApiVersion="v1beta2", PluralName="replicasets")]
    public partial class V1beta2ReplicaSet : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1beta2ReplicaSetSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta2";
        public const string KubeKind = "ReplicaSet";
        public const string KubeGroup = "apps";
    }

    [KubernetesEntity(Group="apps", Kind="ReplicaSetList", ApiVersion="v1beta2", PluralName="replicasets")]
    public partial class V1beta2ReplicaSetList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1beta2ReplicaSet>, IValidate
    {
        public const string KubeApiVersion = "v1beta2";
        public const string KubeKind = "ReplicaSetList";
        public const string KubeGroup = "apps";
    }

    [KubernetesEntity(Group="apps", Kind="Scale", ApiVersion="v1beta2", PluralName=null)]
    public partial class V1beta2Scale : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1beta2ScaleSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta2";
        public const string KubeKind = "Scale";
        public const string KubeGroup = "apps";
    }

    [KubernetesEntity(Group="apps", Kind="StatefulSet", ApiVersion="v1beta2", PluralName="statefulsets")]
    public partial class V1beta2StatefulSet : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1beta2StatefulSetSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta2";
        public const string KubeKind = "StatefulSet";
        public const string KubeGroup = "apps";
    }

    [KubernetesEntity(Group="apps", Kind="StatefulSetList", ApiVersion="v1beta2", PluralName="statefulsets")]
    public partial class V1beta2StatefulSetList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1beta2StatefulSet>, IValidate
    {
        public const string KubeApiVersion = "v1beta2";
        public const string KubeKind = "StatefulSetList";
        public const string KubeGroup = "apps";
    }

    [KubernetesEntity(Group="auditregistration.k8s.io", Kind="AuditSink", ApiVersion="v1alpha1", PluralName="auditsinks")]
    public partial class V1alpha1AuditSink : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1alpha1AuditSinkSpec>, IValidate
    {
        public const string KubeApiVersion = "v1alpha1";
        public const string KubeKind = "AuditSink";
        public const string KubeGroup = "auditregistration.k8s.io";
    }

    [KubernetesEntity(Group="auditregistration.k8s.io", Kind="AuditSinkList", ApiVersion="v1alpha1", PluralName="auditsinks")]
    public partial class V1alpha1AuditSinkList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1alpha1AuditSink>, IValidate
    {
        public const string KubeApiVersion = "v1alpha1";
        public const string KubeKind = "AuditSinkList";
        public const string KubeGroup = "auditregistration.k8s.io";
    }

    [KubernetesEntity(Group="authentication.k8s.io", Kind="TokenRequest", ApiVersion="v1", PluralName=null)]
    public partial class V1TokenRequest : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1TokenRequestSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "TokenRequest";
        public const string KubeGroup = "authentication.k8s.io";
    }

    [KubernetesEntity(Group="authentication.k8s.io", Kind="TokenReview", ApiVersion="v1", PluralName=null)]
    public partial class V1TokenReview : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1TokenReviewSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "TokenReview";
        public const string KubeGroup = "authentication.k8s.io";
    }

    [KubernetesEntity(Group="authentication.k8s.io", Kind="TokenReview", ApiVersion="v1beta1", PluralName=null)]
    public partial class V1beta1TokenReview : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1beta1TokenReviewSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "TokenReview";
        public const string KubeGroup = "authentication.k8s.io";
    }

    [KubernetesEntity(Group="authorization.k8s.io", Kind="LocalSubjectAccessReview", ApiVersion="v1", PluralName=null)]
    public partial class V1LocalSubjectAccessReview : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1SubjectAccessReviewSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "LocalSubjectAccessReview";
        public const string KubeGroup = "authorization.k8s.io";
    }

    [KubernetesEntity(Group="authorization.k8s.io", Kind="SelfSubjectAccessReview", ApiVersion="v1", PluralName=null)]
    public partial class V1SelfSubjectAccessReview : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1SelfSubjectAccessReviewSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "SelfSubjectAccessReview";
        public const string KubeGroup = "authorization.k8s.io";
    }

    [KubernetesEntity(Group="authorization.k8s.io", Kind="SelfSubjectRulesReview", ApiVersion="v1", PluralName=null)]
    public partial class V1SelfSubjectRulesReview : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1SelfSubjectRulesReviewSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "SelfSubjectRulesReview";
        public const string KubeGroup = "authorization.k8s.io";
    }

    [KubernetesEntity(Group="authorization.k8s.io", Kind="SubjectAccessReview", ApiVersion="v1", PluralName=null)]
    public partial class V1SubjectAccessReview : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1SubjectAccessReviewSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "SubjectAccessReview";
        public const string KubeGroup = "authorization.k8s.io";
    }

    [KubernetesEntity(Group="authorization.k8s.io", Kind="LocalSubjectAccessReview", ApiVersion="v1beta1", PluralName=null)]
    public partial class V1beta1LocalSubjectAccessReview : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1beta1SubjectAccessReviewSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "LocalSubjectAccessReview";
        public const string KubeGroup = "authorization.k8s.io";
    }

    [KubernetesEntity(Group="authorization.k8s.io", Kind="SelfSubjectAccessReview", ApiVersion="v1beta1", PluralName=null)]
    public partial class V1beta1SelfSubjectAccessReview : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1beta1SelfSubjectAccessReviewSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "SelfSubjectAccessReview";
        public const string KubeGroup = "authorization.k8s.io";
    }

    [KubernetesEntity(Group="authorization.k8s.io", Kind="SelfSubjectRulesReview", ApiVersion="v1beta1", PluralName=null)]
    public partial class V1beta1SelfSubjectRulesReview : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1beta1SelfSubjectRulesReviewSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "SelfSubjectRulesReview";
        public const string KubeGroup = "authorization.k8s.io";
    }

    [KubernetesEntity(Group="authorization.k8s.io", Kind="SubjectAccessReview", ApiVersion="v1beta1", PluralName=null)]
    public partial class V1beta1SubjectAccessReview : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1beta1SubjectAccessReviewSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "SubjectAccessReview";
        public const string KubeGroup = "authorization.k8s.io";
    }

    [KubernetesEntity(Group="autoscaling", Kind="HorizontalPodAutoscaler", ApiVersion="v1", PluralName="horizontalpodautoscalers")]
    public partial class V1HorizontalPodAutoscaler : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1HorizontalPodAutoscalerSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "HorizontalPodAutoscaler";
        public const string KubeGroup = "autoscaling";
    }

    [KubernetesEntity(Group="autoscaling", Kind="HorizontalPodAutoscalerList", ApiVersion="v1", PluralName="horizontalpodautoscalers")]
    public partial class V1HorizontalPodAutoscalerList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1HorizontalPodAutoscaler>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "HorizontalPodAutoscalerList";
        public const string KubeGroup = "autoscaling";
    }

    [KubernetesEntity(Group="autoscaling", Kind="Scale", ApiVersion="v1", PluralName=null)]
    public partial class V1Scale : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1ScaleSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "Scale";
        public const string KubeGroup = "autoscaling";
    }

    [KubernetesEntity(Group="autoscaling", Kind="HorizontalPodAutoscaler", ApiVersion="v2beta1", PluralName="horizontalpodautoscalers")]
    public partial class V2beta1HorizontalPodAutoscaler : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V2beta1HorizontalPodAutoscalerSpec>, IValidate
    {
        public const string KubeApiVersion = "v2beta1";
        public const string KubeKind = "HorizontalPodAutoscaler";
        public const string KubeGroup = "autoscaling";
    }

    [KubernetesEntity(Group="autoscaling", Kind="HorizontalPodAutoscalerList", ApiVersion="v2beta1", PluralName="horizontalpodautoscalers")]
    public partial class V2beta1HorizontalPodAutoscalerList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V2beta1HorizontalPodAutoscaler>, IValidate
    {
        public const string KubeApiVersion = "v2beta1";
        public const string KubeKind = "HorizontalPodAutoscalerList";
        public const string KubeGroup = "autoscaling";
    }

    [KubernetesEntity(Group="autoscaling", Kind="HorizontalPodAutoscaler", ApiVersion="v2beta2", PluralName="horizontalpodautoscalers")]
    public partial class V2beta2HorizontalPodAutoscaler : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V2beta2HorizontalPodAutoscalerSpec>, IValidate
    {
        public const string KubeApiVersion = "v2beta2";
        public const string KubeKind = "HorizontalPodAutoscaler";
        public const string KubeGroup = "autoscaling";
    }

    [KubernetesEntity(Group="autoscaling", Kind="HorizontalPodAutoscalerList", ApiVersion="v2beta2", PluralName="horizontalpodautoscalers")]
    public partial class V2beta2HorizontalPodAutoscalerList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V2beta2HorizontalPodAutoscaler>, IValidate
    {
        public const string KubeApiVersion = "v2beta2";
        public const string KubeKind = "HorizontalPodAutoscalerList";
        public const string KubeGroup = "autoscaling";
    }

    [KubernetesEntity(Group="batch", Kind="Job", ApiVersion="v1", PluralName="jobs")]
    public partial class V1Job : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1JobSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "Job";
        public const string KubeGroup = "batch";
    }

    [KubernetesEntity(Group="batch", Kind="JobList", ApiVersion="v1", PluralName="jobs")]
    public partial class V1JobList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1Job>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "JobList";
        public const string KubeGroup = "batch";
    }

    [KubernetesEntity(Group="batch", Kind="CronJob", ApiVersion="v1beta1", PluralName="cronjobs")]
    public partial class V1beta1CronJob : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1beta1CronJobSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "CronJob";
        public const string KubeGroup = "batch";
    }

    [KubernetesEntity(Group="batch", Kind="CronJobList", ApiVersion="v1beta1", PluralName="cronjobs")]
    public partial class V1beta1CronJobList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1beta1CronJob>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "CronJobList";
        public const string KubeGroup = "batch";
    }

    [KubernetesEntity(Group="batch", Kind="CronJob", ApiVersion="v2alpha1", PluralName="cronjobs")]
    public partial class V2alpha1CronJob : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V2alpha1CronJobSpec>, IValidate
    {
        public const string KubeApiVersion = "v2alpha1";
        public const string KubeKind = "CronJob";
        public const string KubeGroup = "batch";
    }

    [KubernetesEntity(Group="batch", Kind="CronJobList", ApiVersion="v2alpha1", PluralName="cronjobs")]
    public partial class V2alpha1CronJobList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V2alpha1CronJob>, IValidate
    {
        public const string KubeApiVersion = "v2alpha1";
        public const string KubeKind = "CronJobList";
        public const string KubeGroup = "batch";
    }

    [KubernetesEntity(Group="certificates.k8s.io", Kind="CertificateSigningRequest", ApiVersion="v1beta1", PluralName="certificatesigningrequests")]
    public partial class V1beta1CertificateSigningRequest : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1beta1CertificateSigningRequestSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "CertificateSigningRequest";
        public const string KubeGroup = "certificates.k8s.io";
    }

    [KubernetesEntity(Group="certificates.k8s.io", Kind="CertificateSigningRequestList", ApiVersion="v1beta1", PluralName="certificatesigningrequests")]
    public partial class V1beta1CertificateSigningRequestList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1beta1CertificateSigningRequest>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "CertificateSigningRequestList";
        public const string KubeGroup = "certificates.k8s.io";
    }

    [KubernetesEntity(Group="coordination.k8s.io", Kind="Lease", ApiVersion="v1", PluralName="leases")]
    public partial class V1Lease : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1LeaseSpec>
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "Lease";
        public const string KubeGroup = "coordination.k8s.io";
    }

    [KubernetesEntity(Group="coordination.k8s.io", Kind="LeaseList", ApiVersion="v1", PluralName="leases")]
    public partial class V1LeaseList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1Lease>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "LeaseList";
        public const string KubeGroup = "coordination.k8s.io";
    }

    [KubernetesEntity(Group="coordination.k8s.io", Kind="Lease", ApiVersion="v1beta1", PluralName="leases")]
    public partial class V1beta1Lease : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1beta1LeaseSpec>
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "Lease";
        public const string KubeGroup = "coordination.k8s.io";
    }

    [KubernetesEntity(Group="coordination.k8s.io", Kind="LeaseList", ApiVersion="v1beta1", PluralName="leases")]
    public partial class V1beta1LeaseList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1beta1Lease>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "LeaseList";
        public const string KubeGroup = "coordination.k8s.io";
    }

    [KubernetesEntity(Group="", Kind="Binding", ApiVersion="v1", PluralName=null)]
    public partial class V1Binding : IKubernetesObject, IMetadata<V1ObjectMeta>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "Binding";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="ComponentStatus", ApiVersion="v1", PluralName="componentstatuses")]
    public partial class V1ComponentStatus : IKubernetesObject, IMetadata<V1ObjectMeta>
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "ComponentStatus";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="ComponentStatusList", ApiVersion="v1", PluralName="componentstatuses")]
    public partial class V1ComponentStatusList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1ComponentStatus>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "ComponentStatusList";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="ConfigMap", ApiVersion="v1", PluralName="configmaps")]
    public partial class V1ConfigMap : IKubernetesObject, IMetadata<V1ObjectMeta>
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "ConfigMap";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="ConfigMapList", ApiVersion="v1", PluralName="configmaps")]
    public partial class V1ConfigMapList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1ConfigMap>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "ConfigMapList";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="Endpoints", ApiVersion="v1", PluralName="endpoints")]
    public partial class V1Endpoints : IKubernetesObject, IMetadata<V1ObjectMeta>
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "Endpoints";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="EndpointsList", ApiVersion="v1", PluralName="endpoints")]
    public partial class V1EndpointsList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1Endpoints>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "EndpointsList";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="Event", ApiVersion="v1", PluralName="events")]
    public partial class V1Event : IKubernetesObject, IMetadata<V1ObjectMeta>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "Event";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="EventList", ApiVersion="v1", PluralName="events")]
    public partial class V1EventList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1Event>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "EventList";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="LimitRange", ApiVersion="v1", PluralName="limitranges")]
    public partial class V1LimitRange : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1LimitRangeSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "LimitRange";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="LimitRangeList", ApiVersion="v1", PluralName="limitranges")]
    public partial class V1LimitRangeList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1LimitRange>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "LimitRangeList";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="Namespace", ApiVersion="v1", PluralName="namespaces")]
    public partial class V1Namespace : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1NamespaceSpec>
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "Namespace";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="NamespaceList", ApiVersion="v1", PluralName="namespaces")]
    public partial class V1NamespaceList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1Namespace>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "NamespaceList";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="Node", ApiVersion="v1", PluralName="nodes")]
    public partial class V1Node : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1NodeSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "Node";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="NodeList", ApiVersion="v1", PluralName="nodes")]
    public partial class V1NodeList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1Node>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "NodeList";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="PersistentVolume", ApiVersion="v1", PluralName="persistentvolumes")]
    public partial class V1PersistentVolume : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1PersistentVolumeSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "PersistentVolume";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="PersistentVolumeClaim", ApiVersion="v1", PluralName="persistentvolumeclaims")]
    public partial class V1PersistentVolumeClaim : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1PersistentVolumeClaimSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "PersistentVolumeClaim";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="PersistentVolumeClaimList", ApiVersion="v1", PluralName="persistentvolumeclaims")]
    public partial class V1PersistentVolumeClaimList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1PersistentVolumeClaim>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "PersistentVolumeClaimList";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="PersistentVolumeList", ApiVersion="v1", PluralName="persistentvolumes")]
    public partial class V1PersistentVolumeList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1PersistentVolume>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "PersistentVolumeList";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="Pod", ApiVersion="v1", PluralName="pods")]
    public partial class V1Pod : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1PodSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "Pod";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="PodList", ApiVersion="v1", PluralName="pods")]
    public partial class V1PodList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1Pod>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "PodList";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="PodTemplate", ApiVersion="v1", PluralName="podtemplates")]
    public partial class V1PodTemplate : IKubernetesObject, IMetadata<V1ObjectMeta>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "PodTemplate";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="PodTemplateList", ApiVersion="v1", PluralName="podtemplates")]
    public partial class V1PodTemplateList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1PodTemplate>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "PodTemplateList";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="ReplicationController", ApiVersion="v1", PluralName="replicationcontrollers")]
    public partial class V1ReplicationController : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1ReplicationControllerSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "ReplicationController";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="ReplicationControllerList", ApiVersion="v1", PluralName="replicationcontrollers")]
    public partial class V1ReplicationControllerList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1ReplicationController>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "ReplicationControllerList";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="ResourceQuota", ApiVersion="v1", PluralName="resourcequotas")]
    public partial class V1ResourceQuota : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1ResourceQuotaSpec>
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "ResourceQuota";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="ResourceQuotaList", ApiVersion="v1", PluralName="resourcequotas")]
    public partial class V1ResourceQuotaList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1ResourceQuota>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "ResourceQuotaList";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="Secret", ApiVersion="v1", PluralName="secrets")]
    public partial class V1Secret : IKubernetesObject, IMetadata<V1ObjectMeta>
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "Secret";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="SecretList", ApiVersion="v1", PluralName="secrets")]
    public partial class V1SecretList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1Secret>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "SecretList";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="Service", ApiVersion="v1", PluralName="services")]
    public partial class V1Service : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1ServiceSpec>
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "Service";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="ServiceAccount", ApiVersion="v1", PluralName="serviceaccounts")]
    public partial class V1ServiceAccount : IKubernetesObject, IMetadata<V1ObjectMeta>
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "ServiceAccount";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="ServiceAccountList", ApiVersion="v1", PluralName="serviceaccounts")]
    public partial class V1ServiceAccountList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1ServiceAccount>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "ServiceAccountList";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="ServiceList", ApiVersion="v1", PluralName="services")]
    public partial class V1ServiceList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1Service>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "ServiceList";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="discovery.k8s.io", Kind="EndpointSlice", ApiVersion="v1alpha1", PluralName="endpointslices")]
    public partial class V1alpha1EndpointSlice : IKubernetesObject, IMetadata<V1ObjectMeta>, IValidate
    {
        public const string KubeApiVersion = "v1alpha1";
        public const string KubeKind = "EndpointSlice";
        public const string KubeGroup = "discovery.k8s.io";
    }

    [KubernetesEntity(Group="discovery.k8s.io", Kind="EndpointSliceList", ApiVersion="v1alpha1", PluralName="endpointslices")]
    public partial class V1alpha1EndpointSliceList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1alpha1EndpointSlice>, IValidate
    {
        public const string KubeApiVersion = "v1alpha1";
        public const string KubeKind = "EndpointSliceList";
        public const string KubeGroup = "discovery.k8s.io";
    }

    [KubernetesEntity(Group="events.k8s.io", Kind="Event", ApiVersion="v1beta1", PluralName="events")]
    public partial class V1beta1Event : IKubernetesObject, IMetadata<V1ObjectMeta>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "Event";
        public const string KubeGroup = "events.k8s.io";
    }

    [KubernetesEntity(Group="events.k8s.io", Kind="EventList", ApiVersion="v1beta1", PluralName="events")]
    public partial class V1beta1EventList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1beta1Event>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "EventList";
        public const string KubeGroup = "events.k8s.io";
    }

    [KubernetesEntity(Group="extensions", Kind="DaemonSet", ApiVersion="v1beta1", PluralName="daemonsets")]
    public partial class V1beta1DaemonSet : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1beta1DaemonSetSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "DaemonSet";
        public const string KubeGroup = "extensions";
    }

    [KubernetesEntity(Group="extensions", Kind="DaemonSetList", ApiVersion="v1beta1", PluralName="daemonsets")]
    public partial class V1beta1DaemonSetList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1beta1DaemonSet>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "DaemonSetList";
        public const string KubeGroup = "extensions";
    }

    [KubernetesEntity(Group="extensions", Kind="Deployment", ApiVersion="v1beta1", PluralName="deployments")]
    public partial class Extensionsv1beta1Deployment : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<Extensionsv1beta1DeploymentSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "Deployment";
        public const string KubeGroup = "extensions";
    }

    [KubernetesEntity(Group="extensions", Kind="DeploymentList", ApiVersion="v1beta1", PluralName="deployments")]
    public partial class Extensionsv1beta1DeploymentList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<Extensionsv1beta1Deployment>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "DeploymentList";
        public const string KubeGroup = "extensions";
    }

    [KubernetesEntity(Group="extensions", Kind="DeploymentRollback", ApiVersion="v1beta1", PluralName=null)]
    public partial class Extensionsv1beta1DeploymentRollback : IKubernetesObject, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "DeploymentRollback";
        public const string KubeGroup = "extensions";
    }

    [KubernetesEntity(Group="extensions", Kind="Ingress", ApiVersion="v1beta1", PluralName="ingresses")]
    public partial class Extensionsv1beta1Ingress : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<Extensionsv1beta1IngressSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "Ingress";
        public const string KubeGroup = "extensions";
    }

    [KubernetesEntity(Group="extensions", Kind="IngressList", ApiVersion="v1beta1", PluralName="ingresses")]
    public partial class Extensionsv1beta1IngressList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<Extensionsv1beta1Ingress>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "IngressList";
        public const string KubeGroup = "extensions";
    }

    [KubernetesEntity(Group="extensions", Kind="NetworkPolicy", ApiVersion="v1beta1", PluralName="networkpolicies")]
    public partial class V1beta1NetworkPolicy : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1beta1NetworkPolicySpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "NetworkPolicy";
        public const string KubeGroup = "extensions";
    }

    [KubernetesEntity(Group="extensions", Kind="NetworkPolicyList", ApiVersion="v1beta1", PluralName="networkpolicies")]
    public partial class V1beta1NetworkPolicyList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1beta1NetworkPolicy>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "NetworkPolicyList";
        public const string KubeGroup = "extensions";
    }

    [KubernetesEntity(Group="extensions", Kind="PodSecurityPolicy", ApiVersion="v1beta1", PluralName="podsecuritypolicies")]
    public partial class Extensionsv1beta1PodSecurityPolicy : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<Extensionsv1beta1PodSecurityPolicySpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "PodSecurityPolicy";
        public const string KubeGroup = "extensions";
    }

    [KubernetesEntity(Group="extensions", Kind="PodSecurityPolicyList", ApiVersion="v1beta1", PluralName="podsecuritypolicies")]
    public partial class Extensionsv1beta1PodSecurityPolicyList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<Extensionsv1beta1PodSecurityPolicy>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "PodSecurityPolicyList";
        public const string KubeGroup = "extensions";
    }

    [KubernetesEntity(Group="extensions", Kind="ReplicaSet", ApiVersion="v1beta1", PluralName="replicasets")]
    public partial class V1beta1ReplicaSet : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1beta1ReplicaSetSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "ReplicaSet";
        public const string KubeGroup = "extensions";
    }

    [KubernetesEntity(Group="extensions", Kind="ReplicaSetList", ApiVersion="v1beta1", PluralName="replicasets")]
    public partial class V1beta1ReplicaSetList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1beta1ReplicaSet>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "ReplicaSetList";
        public const string KubeGroup = "extensions";
    }

    [KubernetesEntity(Group="extensions", Kind="Scale", ApiVersion="v1beta1", PluralName=null)]
    public partial class Extensionsv1beta1Scale : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<Extensionsv1beta1ScaleSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "Scale";
        public const string KubeGroup = "extensions";
    }

    [KubernetesEntity(Group="networking.k8s.io", Kind="NetworkPolicy", ApiVersion="v1", PluralName="networkpolicies")]
    public partial class V1NetworkPolicy : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1NetworkPolicySpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "NetworkPolicy";
        public const string KubeGroup = "networking.k8s.io";
    }

    [KubernetesEntity(Group="networking.k8s.io", Kind="NetworkPolicyList", ApiVersion="v1", PluralName="networkpolicies")]
    public partial class V1NetworkPolicyList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1NetworkPolicy>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "NetworkPolicyList";
        public const string KubeGroup = "networking.k8s.io";
    }

    [KubernetesEntity(Group="networking.k8s.io", Kind="Ingress", ApiVersion="v1beta1", PluralName="ingresses")]
    public partial class Networkingv1beta1Ingress : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<Networkingv1beta1IngressSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "Ingress";
        public const string KubeGroup = "networking.k8s.io";
    }

    [KubernetesEntity(Group="networking.k8s.io", Kind="IngressList", ApiVersion="v1beta1", PluralName="ingresses")]
    public partial class Networkingv1beta1IngressList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<Networkingv1beta1Ingress>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "IngressList";
        public const string KubeGroup = "networking.k8s.io";
    }

    [KubernetesEntity(Group="node.k8s.io", Kind="RuntimeClass", ApiVersion="v1alpha1", PluralName="runtimeclasses")]
    public partial class V1alpha1RuntimeClass : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1alpha1RuntimeClassSpec>, IValidate
    {
        public const string KubeApiVersion = "v1alpha1";
        public const string KubeKind = "RuntimeClass";
        public const string KubeGroup = "node.k8s.io";
    }

    [KubernetesEntity(Group="node.k8s.io", Kind="RuntimeClassList", ApiVersion="v1alpha1", PluralName="runtimeclasses")]
    public partial class V1alpha1RuntimeClassList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1alpha1RuntimeClass>, IValidate
    {
        public const string KubeApiVersion = "v1alpha1";
        public const string KubeKind = "RuntimeClassList";
        public const string KubeGroup = "node.k8s.io";
    }

    [KubernetesEntity(Group="node.k8s.io", Kind="RuntimeClass", ApiVersion="v1beta1", PluralName="runtimeclasses")]
    public partial class V1beta1RuntimeClass : IKubernetesObject, IMetadata<V1ObjectMeta>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "RuntimeClass";
        public const string KubeGroup = "node.k8s.io";
    }

    [KubernetesEntity(Group="node.k8s.io", Kind="RuntimeClassList", ApiVersion="v1beta1", PluralName="runtimeclasses")]
    public partial class V1beta1RuntimeClassList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1beta1RuntimeClass>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "RuntimeClassList";
        public const string KubeGroup = "node.k8s.io";
    }

    [KubernetesEntity(Group="policy", Kind="Eviction", ApiVersion="v1beta1", PluralName=null)]
    public partial class V1beta1Eviction : IKubernetesObject, IMetadata<V1ObjectMeta>
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "Eviction";
        public const string KubeGroup = "policy";
    }

    [KubernetesEntity(Group="policy", Kind="PodDisruptionBudget", ApiVersion="v1beta1", PluralName="poddisruptionbudgets")]
    public partial class V1beta1PodDisruptionBudget : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1beta1PodDisruptionBudgetSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "PodDisruptionBudget";
        public const string KubeGroup = "policy";
    }

    [KubernetesEntity(Group="policy", Kind="PodDisruptionBudgetList", ApiVersion="v1beta1", PluralName="poddisruptionbudgets")]
    public partial class V1beta1PodDisruptionBudgetList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1beta1PodDisruptionBudget>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "PodDisruptionBudgetList";
        public const string KubeGroup = "policy";
    }

    [KubernetesEntity(Group="policy", Kind="PodSecurityPolicy", ApiVersion="v1beta1", PluralName="podsecuritypolicies")]
    public partial class Policyv1beta1PodSecurityPolicy : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<Policyv1beta1PodSecurityPolicySpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "PodSecurityPolicy";
        public const string KubeGroup = "policy";
    }

    [KubernetesEntity(Group="policy", Kind="PodSecurityPolicyList", ApiVersion="v1beta1", PluralName="podsecuritypolicies")]
    public partial class Policyv1beta1PodSecurityPolicyList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<Policyv1beta1PodSecurityPolicy>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "PodSecurityPolicyList";
        public const string KubeGroup = "policy";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="ClusterRole", ApiVersion="v1", PluralName="clusterroles")]
    public partial class V1ClusterRole : IKubernetesObject, IMetadata<V1ObjectMeta>
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "ClusterRole";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="ClusterRoleBinding", ApiVersion="v1", PluralName="clusterrolebindings")]
    public partial class V1ClusterRoleBinding : IKubernetesObject, IMetadata<V1ObjectMeta>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "ClusterRoleBinding";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="ClusterRoleBindingList", ApiVersion="v1", PluralName="clusterrolebindings")]
    public partial class V1ClusterRoleBindingList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1ClusterRoleBinding>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "ClusterRoleBindingList";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="ClusterRoleList", ApiVersion="v1", PluralName="clusterroles")]
    public partial class V1ClusterRoleList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1ClusterRole>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "ClusterRoleList";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="Role", ApiVersion="v1", PluralName="roles")]
    public partial class V1Role : IKubernetesObject, IMetadata<V1ObjectMeta>
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "Role";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="RoleBinding", ApiVersion="v1", PluralName="rolebindings")]
    public partial class V1RoleBinding : IKubernetesObject, IMetadata<V1ObjectMeta>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "RoleBinding";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="RoleBindingList", ApiVersion="v1", PluralName="rolebindings")]
    public partial class V1RoleBindingList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1RoleBinding>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "RoleBindingList";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="RoleList", ApiVersion="v1", PluralName="roles")]
    public partial class V1RoleList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1Role>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "RoleList";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="ClusterRole", ApiVersion="v1alpha1", PluralName="clusterroles")]
    public partial class V1alpha1ClusterRole : IKubernetesObject, IMetadata<V1ObjectMeta>
    {
        public const string KubeApiVersion = "v1alpha1";
        public const string KubeKind = "ClusterRole";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="ClusterRoleBinding", ApiVersion="v1alpha1", PluralName="clusterrolebindings")]
    public partial class V1alpha1ClusterRoleBinding : IKubernetesObject, IMetadata<V1ObjectMeta>, IValidate
    {
        public const string KubeApiVersion = "v1alpha1";
        public const string KubeKind = "ClusterRoleBinding";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="ClusterRoleBindingList", ApiVersion="v1alpha1", PluralName="clusterrolebindings")]
    public partial class V1alpha1ClusterRoleBindingList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1alpha1ClusterRoleBinding>, IValidate
    {
        public const string KubeApiVersion = "v1alpha1";
        public const string KubeKind = "ClusterRoleBindingList";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="ClusterRoleList", ApiVersion="v1alpha1", PluralName="clusterroles")]
    public partial class V1alpha1ClusterRoleList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1alpha1ClusterRole>, IValidate
    {
        public const string KubeApiVersion = "v1alpha1";
        public const string KubeKind = "ClusterRoleList";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="Role", ApiVersion="v1alpha1", PluralName="roles")]
    public partial class V1alpha1Role : IKubernetesObject, IMetadata<V1ObjectMeta>
    {
        public const string KubeApiVersion = "v1alpha1";
        public const string KubeKind = "Role";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="RoleBinding", ApiVersion="v1alpha1", PluralName="rolebindings")]
    public partial class V1alpha1RoleBinding : IKubernetesObject, IMetadata<V1ObjectMeta>, IValidate
    {
        public const string KubeApiVersion = "v1alpha1";
        public const string KubeKind = "RoleBinding";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="RoleBindingList", ApiVersion="v1alpha1", PluralName="rolebindings")]
    public partial class V1alpha1RoleBindingList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1alpha1RoleBinding>, IValidate
    {
        public const string KubeApiVersion = "v1alpha1";
        public const string KubeKind = "RoleBindingList";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="RoleList", ApiVersion="v1alpha1", PluralName="roles")]
    public partial class V1alpha1RoleList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1alpha1Role>, IValidate
    {
        public const string KubeApiVersion = "v1alpha1";
        public const string KubeKind = "RoleList";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="ClusterRole", ApiVersion="v1beta1", PluralName="clusterroles")]
    public partial class V1beta1ClusterRole : IKubernetesObject, IMetadata<V1ObjectMeta>
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "ClusterRole";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="ClusterRoleBinding", ApiVersion="v1beta1", PluralName="clusterrolebindings")]
    public partial class V1beta1ClusterRoleBinding : IKubernetesObject, IMetadata<V1ObjectMeta>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "ClusterRoleBinding";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="ClusterRoleBindingList", ApiVersion="v1beta1", PluralName="clusterrolebindings")]
    public partial class V1beta1ClusterRoleBindingList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1beta1ClusterRoleBinding>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "ClusterRoleBindingList";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="ClusterRoleList", ApiVersion="v1beta1", PluralName="clusterroles")]
    public partial class V1beta1ClusterRoleList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1beta1ClusterRole>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "ClusterRoleList";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="Role", ApiVersion="v1beta1", PluralName="roles")]
    public partial class V1beta1Role : IKubernetesObject, IMetadata<V1ObjectMeta>
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "Role";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="RoleBinding", ApiVersion="v1beta1", PluralName="rolebindings")]
    public partial class V1beta1RoleBinding : IKubernetesObject, IMetadata<V1ObjectMeta>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "RoleBinding";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="RoleBindingList", ApiVersion="v1beta1", PluralName="rolebindings")]
    public partial class V1beta1RoleBindingList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1beta1RoleBinding>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "RoleBindingList";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="RoleList", ApiVersion="v1beta1", PluralName="roles")]
    public partial class V1beta1RoleList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1beta1Role>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "RoleList";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="scheduling.k8s.io", Kind="PriorityClass", ApiVersion="v1", PluralName="priorityclasses")]
    public partial class V1PriorityClass : IKubernetesObject, IMetadata<V1ObjectMeta>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "PriorityClass";
        public const string KubeGroup = "scheduling.k8s.io";
    }

    [KubernetesEntity(Group="scheduling.k8s.io", Kind="PriorityClassList", ApiVersion="v1", PluralName="priorityclasses")]
    public partial class V1PriorityClassList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1PriorityClass>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "PriorityClassList";
        public const string KubeGroup = "scheduling.k8s.io";
    }

    [KubernetesEntity(Group="scheduling.k8s.io", Kind="PriorityClass", ApiVersion="v1alpha1", PluralName="priorityclasses")]
    public partial class V1alpha1PriorityClass : IKubernetesObject, IMetadata<V1ObjectMeta>, IValidate
    {
        public const string KubeApiVersion = "v1alpha1";
        public const string KubeKind = "PriorityClass";
        public const string KubeGroup = "scheduling.k8s.io";
    }

    [KubernetesEntity(Group="scheduling.k8s.io", Kind="PriorityClassList", ApiVersion="v1alpha1", PluralName="priorityclasses")]
    public partial class V1alpha1PriorityClassList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1alpha1PriorityClass>, IValidate
    {
        public const string KubeApiVersion = "v1alpha1";
        public const string KubeKind = "PriorityClassList";
        public const string KubeGroup = "scheduling.k8s.io";
    }

    [KubernetesEntity(Group="scheduling.k8s.io", Kind="PriorityClass", ApiVersion="v1beta1", PluralName="priorityclasses")]
    public partial class V1beta1PriorityClass : IKubernetesObject, IMetadata<V1ObjectMeta>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "PriorityClass";
        public const string KubeGroup = "scheduling.k8s.io";
    }

    [KubernetesEntity(Group="scheduling.k8s.io", Kind="PriorityClassList", ApiVersion="v1beta1", PluralName="priorityclasses")]
    public partial class V1beta1PriorityClassList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1beta1PriorityClass>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "PriorityClassList";
        public const string KubeGroup = "scheduling.k8s.io";
    }

    [KubernetesEntity(Group="settings.k8s.io", Kind="PodPreset", ApiVersion="v1alpha1", PluralName="podpresets")]
    public partial class V1alpha1PodPreset : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1alpha1PodPresetSpec>
    {
        public const string KubeApiVersion = "v1alpha1";
        public const string KubeKind = "PodPreset";
        public const string KubeGroup = "settings.k8s.io";
    }

    [KubernetesEntity(Group="settings.k8s.io", Kind="PodPresetList", ApiVersion="v1alpha1", PluralName="podpresets")]
    public partial class V1alpha1PodPresetList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1alpha1PodPreset>, IValidate
    {
        public const string KubeApiVersion = "v1alpha1";
        public const string KubeKind = "PodPresetList";
        public const string KubeGroup = "settings.k8s.io";
    }

    [KubernetesEntity(Group="storage.k8s.io", Kind="StorageClass", ApiVersion="v1", PluralName="storageclasses")]
    public partial class V1StorageClass : IKubernetesObject, IMetadata<V1ObjectMeta>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "StorageClass";
        public const string KubeGroup = "storage.k8s.io";
    }

    [KubernetesEntity(Group="storage.k8s.io", Kind="StorageClassList", ApiVersion="v1", PluralName="storageclasses")]
    public partial class V1StorageClassList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1StorageClass>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "StorageClassList";
        public const string KubeGroup = "storage.k8s.io";
    }

    [KubernetesEntity(Group="storage.k8s.io", Kind="VolumeAttachment", ApiVersion="v1", PluralName="volumeattachments")]
    public partial class V1VolumeAttachment : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1VolumeAttachmentSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "VolumeAttachment";
        public const string KubeGroup = "storage.k8s.io";
    }

    [KubernetesEntity(Group="storage.k8s.io", Kind="VolumeAttachmentList", ApiVersion="v1", PluralName="volumeattachments")]
    public partial class V1VolumeAttachmentList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1VolumeAttachment>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "VolumeAttachmentList";
        public const string KubeGroup = "storage.k8s.io";
    }

    [KubernetesEntity(Group="storage.k8s.io", Kind="VolumeAttachment", ApiVersion="v1alpha1", PluralName="volumeattachments")]
    public partial class V1alpha1VolumeAttachment : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1alpha1VolumeAttachmentSpec>, IValidate
    {
        public const string KubeApiVersion = "v1alpha1";
        public const string KubeKind = "VolumeAttachment";
        public const string KubeGroup = "storage.k8s.io";
    }

    [KubernetesEntity(Group="storage.k8s.io", Kind="VolumeAttachmentList", ApiVersion="v1alpha1", PluralName="volumeattachments")]
    public partial class V1alpha1VolumeAttachmentList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1alpha1VolumeAttachment>, IValidate
    {
        public const string KubeApiVersion = "v1alpha1";
        public const string KubeKind = "VolumeAttachmentList";
        public const string KubeGroup = "storage.k8s.io";
    }

    [KubernetesEntity(Group="storage.k8s.io", Kind="CSIDriver", ApiVersion="v1beta1", PluralName="csidrivers")]
    public partial class V1beta1CSIDriver : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1beta1CSIDriverSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "CSIDriver";
        public const string KubeGroup = "storage.k8s.io";
    }

    [KubernetesEntity(Group="storage.k8s.io", Kind="CSIDriverList", ApiVersion="v1beta1", PluralName="csidrivers")]
    public partial class V1beta1CSIDriverList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1beta1CSIDriver>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "CSIDriverList";
        public const string KubeGroup = "storage.k8s.io";
    }

    [KubernetesEntity(Group="storage.k8s.io", Kind="CSINode", ApiVersion="v1beta1", PluralName="csinodes")]
    public partial class V1beta1CSINode : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1beta1CSINodeSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "CSINode";
        public const string KubeGroup = "storage.k8s.io";
    }

    [KubernetesEntity(Group="storage.k8s.io", Kind="CSINodeList", ApiVersion="v1beta1", PluralName="csinodes")]
    public partial class V1beta1CSINodeList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1beta1CSINode>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "CSINodeList";
        public const string KubeGroup = "storage.k8s.io";
    }

    [KubernetesEntity(Group="storage.k8s.io", Kind="StorageClass", ApiVersion="v1beta1", PluralName="storageclasses")]
    public partial class V1beta1StorageClass : IKubernetesObject, IMetadata<V1ObjectMeta>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "StorageClass";
        public const string KubeGroup = "storage.k8s.io";
    }

    [KubernetesEntity(Group="storage.k8s.io", Kind="StorageClassList", ApiVersion="v1beta1", PluralName="storageclasses")]
    public partial class V1beta1StorageClassList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1beta1StorageClass>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "StorageClassList";
        public const string KubeGroup = "storage.k8s.io";
    }

    [KubernetesEntity(Group="storage.k8s.io", Kind="VolumeAttachment", ApiVersion="v1beta1", PluralName="volumeattachments")]
    public partial class V1beta1VolumeAttachment : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1beta1VolumeAttachmentSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "VolumeAttachment";
        public const string KubeGroup = "storage.k8s.io";
    }

    [KubernetesEntity(Group="storage.k8s.io", Kind="VolumeAttachmentList", ApiVersion="v1beta1", PluralName="volumeattachments")]
    public partial class V1beta1VolumeAttachmentList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1beta1VolumeAttachment>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "VolumeAttachmentList";
        public const string KubeGroup = "storage.k8s.io";
    }

    [KubernetesEntity(Group="apiextensions.k8s.io", Kind="CustomResourceDefinition", ApiVersion="v1", PluralName="customresourcedefinitions")]
    public partial class V1CustomResourceDefinition : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1CustomResourceDefinitionSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "CustomResourceDefinition";
        public const string KubeGroup = "apiextensions.k8s.io";
    }

    [KubernetesEntity(Group="apiextensions.k8s.io", Kind="CustomResourceDefinitionList", ApiVersion="v1", PluralName="customresourcedefinitions")]
    public partial class V1CustomResourceDefinitionList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1CustomResourceDefinition>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "CustomResourceDefinitionList";
        public const string KubeGroup = "apiextensions.k8s.io";
    }

    [KubernetesEntity(Group="apiextensions.k8s.io", Kind="CustomResourceDefinition", ApiVersion="v1beta1", PluralName="customresourcedefinitions")]
    public partial class V1beta1CustomResourceDefinition : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1beta1CustomResourceDefinitionSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "CustomResourceDefinition";
        public const string KubeGroup = "apiextensions.k8s.io";
    }

    [KubernetesEntity(Group="apiextensions.k8s.io", Kind="CustomResourceDefinitionList", ApiVersion="v1beta1", PluralName="customresourcedefinitions")]
    public partial class V1beta1CustomResourceDefinitionList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1beta1CustomResourceDefinition>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "CustomResourceDefinitionList";
        public const string KubeGroup = "apiextensions.k8s.io";
    }

    [KubernetesEntity(Group="", Kind="APIGroup", ApiVersion="v1", PluralName=null)]
    public partial class V1APIGroup : IKubernetesObject, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "APIGroup";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="APIGroupList", ApiVersion="v1", PluralName=null)]
    public partial class V1APIGroupList : IKubernetesObject, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "APIGroupList";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="APIResourceList", ApiVersion="v1", PluralName=null)]
    public partial class V1APIResourceList : IKubernetesObject, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "APIResourceList";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="APIVersions", ApiVersion="v1", PluralName=null)]
    public partial class V1APIVersions : IKubernetesObject, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "APIVersions";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="DeleteOptions", ApiVersion="v1", PluralName=null)]
    public partial class V1DeleteOptions : IKubernetesObject
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "DeleteOptions";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="Status", ApiVersion="v1", PluralName=null)]
    public partial class V1Status : IKubernetesObject, IMetadata<V1ListMeta>
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "Status";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="apiregistration.k8s.io", Kind="APIService", ApiVersion="v1", PluralName="apiservices")]
    public partial class V1APIService : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1APIServiceSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "APIService";
        public const string KubeGroup = "apiregistration.k8s.io";
    }

    [KubernetesEntity(Group="apiregistration.k8s.io", Kind="APIServiceList", ApiVersion="v1", PluralName="apiservices")]
    public partial class V1APIServiceList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1APIService>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "APIServiceList";
        public const string KubeGroup = "apiregistration.k8s.io";
    }

    [KubernetesEntity(Group="apiregistration.k8s.io", Kind="APIService", ApiVersion="v1beta1", PluralName="apiservices")]
    public partial class V1beta1APIService : IKubernetesObject, IMetadata<V1ObjectMeta>, ISpec<V1beta1APIServiceSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "APIService";
        public const string KubeGroup = "apiregistration.k8s.io";
    }

    [KubernetesEntity(Group="apiregistration.k8s.io", Kind="APIServiceList", ApiVersion="v1beta1", PluralName="apiservices")]
    public partial class V1beta1APIServiceList : IKubernetesObject, IMetadata<V1ListMeta>, IItems<V1beta1APIService>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "APIServiceList";
        public const string KubeGroup = "apiregistration.k8s.io";
    }

}
