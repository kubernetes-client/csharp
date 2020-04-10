namespace k8s.Models
{
    [KubernetesEntity(Group="admissionregistration.k8s.io", Kind="MutatingWebhookConfiguration", ApiVersion="v1", PluralName="mutatingwebhookconfigurations")]
    public partial class V1MutatingWebhookConfiguration : IKubernetesObject<V1ObjectMeta>
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "MutatingWebhookConfiguration";
        public const string KubeGroup = "admissionregistration.k8s.io";
    }

    [KubernetesEntity(Group="admissionregistration.k8s.io", Kind="MutatingWebhookConfigurationList", ApiVersion="v1", PluralName="mutatingwebhookconfigurations")]
    public partial class V1MutatingWebhookConfigurationList : IKubernetesObject<V1ListMeta>, IItems<V1MutatingWebhookConfiguration>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "MutatingWebhookConfigurationList";
        public const string KubeGroup = "admissionregistration.k8s.io";
    }

    [KubernetesEntity(Group="admissionregistration.k8s.io", Kind="ValidatingWebhookConfiguration", ApiVersion="v1", PluralName="validatingwebhookconfigurations")]
    public partial class V1ValidatingWebhookConfiguration : IKubernetesObject<V1ObjectMeta>
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "ValidatingWebhookConfiguration";
        public const string KubeGroup = "admissionregistration.k8s.io";
    }

    [KubernetesEntity(Group="admissionregistration.k8s.io", Kind="ValidatingWebhookConfigurationList", ApiVersion="v1", PluralName="validatingwebhookconfigurations")]
    public partial class V1ValidatingWebhookConfigurationList : IKubernetesObject<V1ListMeta>, IItems<V1ValidatingWebhookConfiguration>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "ValidatingWebhookConfigurationList";
        public const string KubeGroup = "admissionregistration.k8s.io";
    }

    [KubernetesEntity(Group="admissionregistration.k8s.io", Kind="MutatingWebhookConfiguration", ApiVersion="v1beta1", PluralName="mutatingwebhookconfigurations")]
    public partial class V1beta1MutatingWebhookConfiguration : IKubernetesObject<V1ObjectMeta>
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "MutatingWebhookConfiguration";
        public const string KubeGroup = "admissionregistration.k8s.io";
    }

    [KubernetesEntity(Group="admissionregistration.k8s.io", Kind="MutatingWebhookConfigurationList", ApiVersion="v1beta1", PluralName="mutatingwebhookconfigurations")]
    public partial class V1beta1MutatingWebhookConfigurationList : IKubernetesObject<V1ListMeta>, IItems<V1beta1MutatingWebhookConfiguration>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "MutatingWebhookConfigurationList";
        public const string KubeGroup = "admissionregistration.k8s.io";
    }

    [KubernetesEntity(Group="admissionregistration.k8s.io", Kind="ValidatingWebhookConfiguration", ApiVersion="v1beta1", PluralName="validatingwebhookconfigurations")]
    public partial class V1beta1ValidatingWebhookConfiguration : IKubernetesObject<V1ObjectMeta>
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "ValidatingWebhookConfiguration";
        public const string KubeGroup = "admissionregistration.k8s.io";
    }

    [KubernetesEntity(Group="admissionregistration.k8s.io", Kind="ValidatingWebhookConfigurationList", ApiVersion="v1beta1", PluralName="validatingwebhookconfigurations")]
    public partial class V1beta1ValidatingWebhookConfigurationList : IKubernetesObject<V1ListMeta>, IItems<V1beta1ValidatingWebhookConfiguration>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "ValidatingWebhookConfigurationList";
        public const string KubeGroup = "admissionregistration.k8s.io";
    }

    [KubernetesEntity(Group="apps", Kind="ControllerRevision", ApiVersion="v1", PluralName="controllerrevisions")]
    public partial class V1ControllerRevision : IKubernetesObject<V1ObjectMeta>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "ControllerRevision";
        public const string KubeGroup = "apps";
    }

    [KubernetesEntity(Group="apps", Kind="ControllerRevisionList", ApiVersion="v1", PluralName="controllerrevisions")]
    public partial class V1ControllerRevisionList : IKubernetesObject<V1ListMeta>, IItems<V1ControllerRevision>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "ControllerRevisionList";
        public const string KubeGroup = "apps";
    }

    [KubernetesEntity(Group="apps", Kind="DaemonSet", ApiVersion="v1", PluralName="daemonsets")]
    public partial class V1DaemonSet : IKubernetesObject<V1ObjectMeta>, ISpec<V1DaemonSetSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "DaemonSet";
        public const string KubeGroup = "apps";
    }

    [KubernetesEntity(Group="apps", Kind="DaemonSetList", ApiVersion="v1", PluralName="daemonsets")]
    public partial class V1DaemonSetList : IKubernetesObject<V1ListMeta>, IItems<V1DaemonSet>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "DaemonSetList";
        public const string KubeGroup = "apps";
    }

    [KubernetesEntity(Group="apps", Kind="Deployment", ApiVersion="v1", PluralName="deployments")]
    public partial class V1Deployment : IKubernetesObject<V1ObjectMeta>, ISpec<V1DeploymentSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "Deployment";
        public const string KubeGroup = "apps";
    }

    [KubernetesEntity(Group="apps", Kind="DeploymentList", ApiVersion="v1", PluralName="deployments")]
    public partial class V1DeploymentList : IKubernetesObject<V1ListMeta>, IItems<V1Deployment>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "DeploymentList";
        public const string KubeGroup = "apps";
    }

    [KubernetesEntity(Group="apps", Kind="ReplicaSet", ApiVersion="v1", PluralName="replicasets")]
    public partial class V1ReplicaSet : IKubernetesObject<V1ObjectMeta>, ISpec<V1ReplicaSetSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "ReplicaSet";
        public const string KubeGroup = "apps";
    }

    [KubernetesEntity(Group="apps", Kind="ReplicaSetList", ApiVersion="v1", PluralName="replicasets")]
    public partial class V1ReplicaSetList : IKubernetesObject<V1ListMeta>, IItems<V1ReplicaSet>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "ReplicaSetList";
        public const string KubeGroup = "apps";
    }

    [KubernetesEntity(Group="apps", Kind="StatefulSet", ApiVersion="v1", PluralName="statefulsets")]
    public partial class V1StatefulSet : IKubernetesObject<V1ObjectMeta>, ISpec<V1StatefulSetSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "StatefulSet";
        public const string KubeGroup = "apps";
    }

    [KubernetesEntity(Group="apps", Kind="StatefulSetList", ApiVersion="v1", PluralName="statefulsets")]
    public partial class V1StatefulSetList : IKubernetesObject<V1ListMeta>, IItems<V1StatefulSet>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "StatefulSetList";
        public const string KubeGroup = "apps";
    }

    [KubernetesEntity(Group="auditregistration.k8s.io", Kind="AuditSink", ApiVersion="v1alpha1", PluralName="auditsinks")]
    public partial class V1alpha1AuditSink : IKubernetesObject<V1ObjectMeta>, ISpec<V1alpha1AuditSinkSpec>, IValidate
    {
        public const string KubeApiVersion = "v1alpha1";
        public const string KubeKind = "AuditSink";
        public const string KubeGroup = "auditregistration.k8s.io";
    }

    [KubernetesEntity(Group="auditregistration.k8s.io", Kind="AuditSinkList", ApiVersion="v1alpha1", PluralName="auditsinks")]
    public partial class V1alpha1AuditSinkList : IKubernetesObject<V1ListMeta>, IItems<V1alpha1AuditSink>, IValidate
    {
        public const string KubeApiVersion = "v1alpha1";
        public const string KubeKind = "AuditSinkList";
        public const string KubeGroup = "auditregistration.k8s.io";
    }

    [KubernetesEntity(Group="authentication.k8s.io", Kind="TokenRequest", ApiVersion="v1", PluralName=null)]
    public partial class V1TokenRequest : IKubernetesObject<V1ObjectMeta>, ISpec<V1TokenRequestSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "TokenRequest";
        public const string KubeGroup = "authentication.k8s.io";
    }

    [KubernetesEntity(Group="authentication.k8s.io", Kind="TokenReview", ApiVersion="v1", PluralName=null)]
    public partial class V1TokenReview : IKubernetesObject<V1ObjectMeta>, ISpec<V1TokenReviewSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "TokenReview";
        public const string KubeGroup = "authentication.k8s.io";
    }

    [KubernetesEntity(Group="authentication.k8s.io", Kind="TokenReview", ApiVersion="v1beta1", PluralName=null)]
    public partial class V1beta1TokenReview : IKubernetesObject<V1ObjectMeta>, ISpec<V1beta1TokenReviewSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "TokenReview";
        public const string KubeGroup = "authentication.k8s.io";
    }

    [KubernetesEntity(Group="authorization.k8s.io", Kind="LocalSubjectAccessReview", ApiVersion="v1", PluralName=null)]
    public partial class V1LocalSubjectAccessReview : IKubernetesObject<V1ObjectMeta>, ISpec<V1SubjectAccessReviewSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "LocalSubjectAccessReview";
        public const string KubeGroup = "authorization.k8s.io";
    }

    [KubernetesEntity(Group="authorization.k8s.io", Kind="SelfSubjectAccessReview", ApiVersion="v1", PluralName=null)]
    public partial class V1SelfSubjectAccessReview : IKubernetesObject<V1ObjectMeta>, ISpec<V1SelfSubjectAccessReviewSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "SelfSubjectAccessReview";
        public const string KubeGroup = "authorization.k8s.io";
    }

    [KubernetesEntity(Group="authorization.k8s.io", Kind="SelfSubjectRulesReview", ApiVersion="v1", PluralName=null)]
    public partial class V1SelfSubjectRulesReview : IKubernetesObject<V1ObjectMeta>, ISpec<V1SelfSubjectRulesReviewSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "SelfSubjectRulesReview";
        public const string KubeGroup = "authorization.k8s.io";
    }

    [KubernetesEntity(Group="authorization.k8s.io", Kind="SubjectAccessReview", ApiVersion="v1", PluralName=null)]
    public partial class V1SubjectAccessReview : IKubernetesObject<V1ObjectMeta>, ISpec<V1SubjectAccessReviewSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "SubjectAccessReview";
        public const string KubeGroup = "authorization.k8s.io";
    }

    [KubernetesEntity(Group="authorization.k8s.io", Kind="LocalSubjectAccessReview", ApiVersion="v1beta1", PluralName=null)]
    public partial class V1beta1LocalSubjectAccessReview : IKubernetesObject<V1ObjectMeta>, ISpec<V1beta1SubjectAccessReviewSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "LocalSubjectAccessReview";
        public const string KubeGroup = "authorization.k8s.io";
    }

    [KubernetesEntity(Group="authorization.k8s.io", Kind="SelfSubjectAccessReview", ApiVersion="v1beta1", PluralName=null)]
    public partial class V1beta1SelfSubjectAccessReview : IKubernetesObject<V1ObjectMeta>, ISpec<V1beta1SelfSubjectAccessReviewSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "SelfSubjectAccessReview";
        public const string KubeGroup = "authorization.k8s.io";
    }

    [KubernetesEntity(Group="authorization.k8s.io", Kind="SelfSubjectRulesReview", ApiVersion="v1beta1", PluralName=null)]
    public partial class V1beta1SelfSubjectRulesReview : IKubernetesObject<V1ObjectMeta>, ISpec<V1beta1SelfSubjectRulesReviewSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "SelfSubjectRulesReview";
        public const string KubeGroup = "authorization.k8s.io";
    }

    [KubernetesEntity(Group="authorization.k8s.io", Kind="SubjectAccessReview", ApiVersion="v1beta1", PluralName=null)]
    public partial class V1beta1SubjectAccessReview : IKubernetesObject<V1ObjectMeta>, ISpec<V1beta1SubjectAccessReviewSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "SubjectAccessReview";
        public const string KubeGroup = "authorization.k8s.io";
    }

    [KubernetesEntity(Group="autoscaling", Kind="HorizontalPodAutoscaler", ApiVersion="v1", PluralName="horizontalpodautoscalers")]
    public partial class V1HorizontalPodAutoscaler : IKubernetesObject<V1ObjectMeta>, ISpec<V1HorizontalPodAutoscalerSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "HorizontalPodAutoscaler";
        public const string KubeGroup = "autoscaling";
    }

    [KubernetesEntity(Group="autoscaling", Kind="HorizontalPodAutoscalerList", ApiVersion="v1", PluralName="horizontalpodautoscalers")]
    public partial class V1HorizontalPodAutoscalerList : IKubernetesObject<V1ListMeta>, IItems<V1HorizontalPodAutoscaler>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "HorizontalPodAutoscalerList";
        public const string KubeGroup = "autoscaling";
    }

    [KubernetesEntity(Group="autoscaling", Kind="Scale", ApiVersion="v1", PluralName=null)]
    public partial class V1Scale : IKubernetesObject<V1ObjectMeta>, ISpec<V1ScaleSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "Scale";
        public const string KubeGroup = "autoscaling";
    }

    [KubernetesEntity(Group="autoscaling", Kind="HorizontalPodAutoscaler", ApiVersion="v2beta1", PluralName="horizontalpodautoscalers")]
    public partial class V2beta1HorizontalPodAutoscaler : IKubernetesObject<V1ObjectMeta>, ISpec<V2beta1HorizontalPodAutoscalerSpec>, IValidate
    {
        public const string KubeApiVersion = "v2beta1";
        public const string KubeKind = "HorizontalPodAutoscaler";
        public const string KubeGroup = "autoscaling";
    }

    [KubernetesEntity(Group="autoscaling", Kind="HorizontalPodAutoscalerList", ApiVersion="v2beta1", PluralName="horizontalpodautoscalers")]
    public partial class V2beta1HorizontalPodAutoscalerList : IKubernetesObject<V1ListMeta>, IItems<V2beta1HorizontalPodAutoscaler>, IValidate
    {
        public const string KubeApiVersion = "v2beta1";
        public const string KubeKind = "HorizontalPodAutoscalerList";
        public const string KubeGroup = "autoscaling";
    }

    [KubernetesEntity(Group="autoscaling", Kind="HorizontalPodAutoscaler", ApiVersion="v2beta2", PluralName="horizontalpodautoscalers")]
    public partial class V2beta2HorizontalPodAutoscaler : IKubernetesObject<V1ObjectMeta>, ISpec<V2beta2HorizontalPodAutoscalerSpec>, IValidate
    {
        public const string KubeApiVersion = "v2beta2";
        public const string KubeKind = "HorizontalPodAutoscaler";
        public const string KubeGroup = "autoscaling";
    }

    [KubernetesEntity(Group="autoscaling", Kind="HorizontalPodAutoscalerList", ApiVersion="v2beta2", PluralName="horizontalpodautoscalers")]
    public partial class V2beta2HorizontalPodAutoscalerList : IKubernetesObject<V1ListMeta>, IItems<V2beta2HorizontalPodAutoscaler>, IValidate
    {
        public const string KubeApiVersion = "v2beta2";
        public const string KubeKind = "HorizontalPodAutoscalerList";
        public const string KubeGroup = "autoscaling";
    }

    [KubernetesEntity(Group="batch", Kind="Job", ApiVersion="v1", PluralName="jobs")]
    public partial class V1Job : IKubernetesObject<V1ObjectMeta>, ISpec<V1JobSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "Job";
        public const string KubeGroup = "batch";
    }

    [KubernetesEntity(Group="batch", Kind="JobList", ApiVersion="v1", PluralName="jobs")]
    public partial class V1JobList : IKubernetesObject<V1ListMeta>, IItems<V1Job>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "JobList";
        public const string KubeGroup = "batch";
    }

    [KubernetesEntity(Group="batch", Kind="CronJob", ApiVersion="v1beta1", PluralName="cronjobs")]
    public partial class V1beta1CronJob : IKubernetesObject<V1ObjectMeta>, ISpec<V1beta1CronJobSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "CronJob";
        public const string KubeGroup = "batch";
    }

    [KubernetesEntity(Group="batch", Kind="CronJobList", ApiVersion="v1beta1", PluralName="cronjobs")]
    public partial class V1beta1CronJobList : IKubernetesObject<V1ListMeta>, IItems<V1beta1CronJob>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "CronJobList";
        public const string KubeGroup = "batch";
    }

    [KubernetesEntity(Group="batch", Kind="CronJob", ApiVersion="v2alpha1", PluralName="cronjobs")]
    public partial class V2alpha1CronJob : IKubernetesObject<V1ObjectMeta>, ISpec<V2alpha1CronJobSpec>, IValidate
    {
        public const string KubeApiVersion = "v2alpha1";
        public const string KubeKind = "CronJob";
        public const string KubeGroup = "batch";
    }

    [KubernetesEntity(Group="batch", Kind="CronJobList", ApiVersion="v2alpha1", PluralName="cronjobs")]
    public partial class V2alpha1CronJobList : IKubernetesObject<V1ListMeta>, IItems<V2alpha1CronJob>, IValidate
    {
        public const string KubeApiVersion = "v2alpha1";
        public const string KubeKind = "CronJobList";
        public const string KubeGroup = "batch";
    }

    [KubernetesEntity(Group="certificates.k8s.io", Kind="CertificateSigningRequest", ApiVersion="v1beta1", PluralName="certificatesigningrequests")]
    public partial class V1beta1CertificateSigningRequest : IKubernetesObject<V1ObjectMeta>, ISpec<V1beta1CertificateSigningRequestSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "CertificateSigningRequest";
        public const string KubeGroup = "certificates.k8s.io";
    }

    [KubernetesEntity(Group="certificates.k8s.io", Kind="CertificateSigningRequestList", ApiVersion="v1beta1", PluralName="certificatesigningrequests")]
    public partial class V1beta1CertificateSigningRequestList : IKubernetesObject<V1ListMeta>, IItems<V1beta1CertificateSigningRequest>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "CertificateSigningRequestList";
        public const string KubeGroup = "certificates.k8s.io";
    }

    [KubernetesEntity(Group="coordination.k8s.io", Kind="Lease", ApiVersion="v1", PluralName="leases")]
    public partial class V1Lease : IKubernetesObject<V1ObjectMeta>, ISpec<V1LeaseSpec>
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "Lease";
        public const string KubeGroup = "coordination.k8s.io";
    }

    [KubernetesEntity(Group="coordination.k8s.io", Kind="LeaseList", ApiVersion="v1", PluralName="leases")]
    public partial class V1LeaseList : IKubernetesObject<V1ListMeta>, IItems<V1Lease>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "LeaseList";
        public const string KubeGroup = "coordination.k8s.io";
    }

    [KubernetesEntity(Group="coordination.k8s.io", Kind="Lease", ApiVersion="v1beta1", PluralName="leases")]
    public partial class V1beta1Lease : IKubernetesObject<V1ObjectMeta>, ISpec<V1beta1LeaseSpec>
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "Lease";
        public const string KubeGroup = "coordination.k8s.io";
    }

    [KubernetesEntity(Group="coordination.k8s.io", Kind="LeaseList", ApiVersion="v1beta1", PluralName="leases")]
    public partial class V1beta1LeaseList : IKubernetesObject<V1ListMeta>, IItems<V1beta1Lease>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "LeaseList";
        public const string KubeGroup = "coordination.k8s.io";
    }

    [KubernetesEntity(Group="", Kind="Binding", ApiVersion="v1", PluralName=null)]
    public partial class V1Binding : IKubernetesObject<V1ObjectMeta>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "Binding";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="ComponentStatus", ApiVersion="v1", PluralName="componentstatuses")]
    public partial class V1ComponentStatus : IKubernetesObject<V1ObjectMeta>
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "ComponentStatus";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="ComponentStatusList", ApiVersion="v1", PluralName="componentstatuses")]
    public partial class V1ComponentStatusList : IKubernetesObject<V1ListMeta>, IItems<V1ComponentStatus>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "ComponentStatusList";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="ConfigMap", ApiVersion="v1", PluralName="configmaps")]
    public partial class V1ConfigMap : IKubernetesObject<V1ObjectMeta>
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "ConfigMap";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="ConfigMapList", ApiVersion="v1", PluralName="configmaps")]
    public partial class V1ConfigMapList : IKubernetesObject<V1ListMeta>, IItems<V1ConfigMap>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "ConfigMapList";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="Endpoints", ApiVersion="v1", PluralName="endpoints")]
    public partial class V1Endpoints : IKubernetesObject<V1ObjectMeta>
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "Endpoints";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="EndpointsList", ApiVersion="v1", PluralName="endpoints")]
    public partial class V1EndpointsList : IKubernetesObject<V1ListMeta>, IItems<V1Endpoints>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "EndpointsList";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="Event", ApiVersion="v1", PluralName="events")]
    public partial class V1Event : IKubernetesObject<V1ObjectMeta>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "Event";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="EventList", ApiVersion="v1", PluralName="events")]
    public partial class V1EventList : IKubernetesObject<V1ListMeta>, IItems<V1Event>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "EventList";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="LimitRange", ApiVersion="v1", PluralName="limitranges")]
    public partial class V1LimitRange : IKubernetesObject<V1ObjectMeta>, ISpec<V1LimitRangeSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "LimitRange";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="LimitRangeList", ApiVersion="v1", PluralName="limitranges")]
    public partial class V1LimitRangeList : IKubernetesObject<V1ListMeta>, IItems<V1LimitRange>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "LimitRangeList";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="Namespace", ApiVersion="v1", PluralName="namespaces")]
    public partial class V1Namespace : IKubernetesObject<V1ObjectMeta>, ISpec<V1NamespaceSpec>
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "Namespace";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="NamespaceList", ApiVersion="v1", PluralName="namespaces")]
    public partial class V1NamespaceList : IKubernetesObject<V1ListMeta>, IItems<V1Namespace>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "NamespaceList";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="Node", ApiVersion="v1", PluralName="nodes")]
    public partial class V1Node : IKubernetesObject<V1ObjectMeta>, ISpec<V1NodeSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "Node";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="NodeList", ApiVersion="v1", PluralName="nodes")]
    public partial class V1NodeList : IKubernetesObject<V1ListMeta>, IItems<V1Node>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "NodeList";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="PersistentVolume", ApiVersion="v1", PluralName="persistentvolumes")]
    public partial class V1PersistentVolume : IKubernetesObject<V1ObjectMeta>, ISpec<V1PersistentVolumeSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "PersistentVolume";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="PersistentVolumeClaim", ApiVersion="v1", PluralName="persistentvolumeclaims")]
    public partial class V1PersistentVolumeClaim : IKubernetesObject<V1ObjectMeta>, ISpec<V1PersistentVolumeClaimSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "PersistentVolumeClaim";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="PersistentVolumeClaimList", ApiVersion="v1", PluralName="persistentvolumeclaims")]
    public partial class V1PersistentVolumeClaimList : IKubernetesObject<V1ListMeta>, IItems<V1PersistentVolumeClaim>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "PersistentVolumeClaimList";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="PersistentVolumeList", ApiVersion="v1", PluralName="persistentvolumes")]
    public partial class V1PersistentVolumeList : IKubernetesObject<V1ListMeta>, IItems<V1PersistentVolume>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "PersistentVolumeList";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="Pod", ApiVersion="v1", PluralName="pods")]
    public partial class V1Pod : IKubernetesObject<V1ObjectMeta>, ISpec<V1PodSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "Pod";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="PodList", ApiVersion="v1", PluralName="pods")]
    public partial class V1PodList : IKubernetesObject<V1ListMeta>, IItems<V1Pod>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "PodList";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="PodTemplate", ApiVersion="v1", PluralName="podtemplates")]
    public partial class V1PodTemplate : IKubernetesObject<V1ObjectMeta>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "PodTemplate";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="PodTemplateList", ApiVersion="v1", PluralName="podtemplates")]
    public partial class V1PodTemplateList : IKubernetesObject<V1ListMeta>, IItems<V1PodTemplate>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "PodTemplateList";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="ReplicationController", ApiVersion="v1", PluralName="replicationcontrollers")]
    public partial class V1ReplicationController : IKubernetesObject<V1ObjectMeta>, ISpec<V1ReplicationControllerSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "ReplicationController";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="ReplicationControllerList", ApiVersion="v1", PluralName="replicationcontrollers")]
    public partial class V1ReplicationControllerList : IKubernetesObject<V1ListMeta>, IItems<V1ReplicationController>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "ReplicationControllerList";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="ResourceQuota", ApiVersion="v1", PluralName="resourcequotas")]
    public partial class V1ResourceQuota : IKubernetesObject<V1ObjectMeta>, ISpec<V1ResourceQuotaSpec>
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "ResourceQuota";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="ResourceQuotaList", ApiVersion="v1", PluralName="resourcequotas")]
    public partial class V1ResourceQuotaList : IKubernetesObject<V1ListMeta>, IItems<V1ResourceQuota>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "ResourceQuotaList";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="Secret", ApiVersion="v1", PluralName="secrets")]
    public partial class V1Secret : IKubernetesObject<V1ObjectMeta>
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "Secret";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="SecretList", ApiVersion="v1", PluralName="secrets")]
    public partial class V1SecretList : IKubernetesObject<V1ListMeta>, IItems<V1Secret>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "SecretList";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="Service", ApiVersion="v1", PluralName="services")]
    public partial class V1Service : IKubernetesObject<V1ObjectMeta>, ISpec<V1ServiceSpec>
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "Service";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="ServiceAccount", ApiVersion="v1", PluralName="serviceaccounts")]
    public partial class V1ServiceAccount : IKubernetesObject<V1ObjectMeta>
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "ServiceAccount";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="ServiceAccountList", ApiVersion="v1", PluralName="serviceaccounts")]
    public partial class V1ServiceAccountList : IKubernetesObject<V1ListMeta>, IItems<V1ServiceAccount>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "ServiceAccountList";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="", Kind="ServiceList", ApiVersion="v1", PluralName="services")]
    public partial class V1ServiceList : IKubernetesObject<V1ListMeta>, IItems<V1Service>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "ServiceList";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="discovery.k8s.io", Kind="EndpointSlice", ApiVersion="v1beta1", PluralName="endpointslices")]
    public partial class V1beta1EndpointSlice : IKubernetesObject<V1ObjectMeta>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "EndpointSlice";
        public const string KubeGroup = "discovery.k8s.io";
    }

    [KubernetesEntity(Group="discovery.k8s.io", Kind="EndpointSliceList", ApiVersion="v1beta1", PluralName="endpointslices")]
    public partial class V1beta1EndpointSliceList : IKubernetesObject<V1ListMeta>, IItems<V1beta1EndpointSlice>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "EndpointSliceList";
        public const string KubeGroup = "discovery.k8s.io";
    }

    [KubernetesEntity(Group="events.k8s.io", Kind="Event", ApiVersion="v1beta1", PluralName="events")]
    public partial class V1beta1Event : IKubernetesObject<V1ObjectMeta>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "Event";
        public const string KubeGroup = "events.k8s.io";
    }

    [KubernetesEntity(Group="events.k8s.io", Kind="EventList", ApiVersion="v1beta1", PluralName="events")]
    public partial class V1beta1EventList : IKubernetesObject<V1ListMeta>, IItems<V1beta1Event>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "EventList";
        public const string KubeGroup = "events.k8s.io";
    }

    [KubernetesEntity(Group="extensions", Kind="Ingress", ApiVersion="v1beta1", PluralName="ingresses")]
    public partial class Extensionsv1beta1Ingress : IKubernetesObject<V1ObjectMeta>, ISpec<Extensionsv1beta1IngressSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "Ingress";
        public const string KubeGroup = "extensions";
    }

    [KubernetesEntity(Group="extensions", Kind="IngressList", ApiVersion="v1beta1", PluralName="ingresses")]
    public partial class Extensionsv1beta1IngressList : IKubernetesObject<V1ListMeta>, IItems<Extensionsv1beta1Ingress>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "IngressList";
        public const string KubeGroup = "extensions";
    }

    [KubernetesEntity(Group="flowcontrol.apiserver.k8s.io", Kind="FlowSchema", ApiVersion="v1alpha1", PluralName="flowschemas")]
    public partial class V1alpha1FlowSchema : IKubernetesObject<V1ObjectMeta>, ISpec<V1alpha1FlowSchemaSpec>, IValidate
    {
        public const string KubeApiVersion = "v1alpha1";
        public const string KubeKind = "FlowSchema";
        public const string KubeGroup = "flowcontrol.apiserver.k8s.io";
    }

    [KubernetesEntity(Group="flowcontrol.apiserver.k8s.io", Kind="FlowSchemaList", ApiVersion="v1alpha1", PluralName="flowschemas")]
    public partial class V1alpha1FlowSchemaList : IKubernetesObject<V1ListMeta>, IItems<V1alpha1FlowSchema>, IValidate
    {
        public const string KubeApiVersion = "v1alpha1";
        public const string KubeKind = "FlowSchemaList";
        public const string KubeGroup = "flowcontrol.apiserver.k8s.io";
    }

    [KubernetesEntity(Group="flowcontrol.apiserver.k8s.io", Kind="PriorityLevelConfiguration", ApiVersion="v1alpha1", PluralName="prioritylevelconfigurations")]
    public partial class V1alpha1PriorityLevelConfiguration : IKubernetesObject<V1ObjectMeta>, ISpec<V1alpha1PriorityLevelConfigurationSpec>, IValidate
    {
        public const string KubeApiVersion = "v1alpha1";
        public const string KubeKind = "PriorityLevelConfiguration";
        public const string KubeGroup = "flowcontrol.apiserver.k8s.io";
    }

    [KubernetesEntity(Group="flowcontrol.apiserver.k8s.io", Kind="PriorityLevelConfigurationList", ApiVersion="v1alpha1", PluralName="prioritylevelconfigurations")]
    public partial class V1alpha1PriorityLevelConfigurationList : IKubernetesObject<V1ListMeta>, IItems<V1alpha1PriorityLevelConfiguration>, IValidate
    {
        public const string KubeApiVersion = "v1alpha1";
        public const string KubeKind = "PriorityLevelConfigurationList";
        public const string KubeGroup = "flowcontrol.apiserver.k8s.io";
    }

    [KubernetesEntity(Group="networking.k8s.io", Kind="NetworkPolicy", ApiVersion="v1", PluralName="networkpolicies")]
    public partial class V1NetworkPolicy : IKubernetesObject<V1ObjectMeta>, ISpec<V1NetworkPolicySpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "NetworkPolicy";
        public const string KubeGroup = "networking.k8s.io";
    }

    [KubernetesEntity(Group="networking.k8s.io", Kind="NetworkPolicyList", ApiVersion="v1", PluralName="networkpolicies")]
    public partial class V1NetworkPolicyList : IKubernetesObject<V1ListMeta>, IItems<V1NetworkPolicy>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "NetworkPolicyList";
        public const string KubeGroup = "networking.k8s.io";
    }

    [KubernetesEntity(Group="networking.k8s.io", Kind="Ingress", ApiVersion="v1beta1", PluralName="ingresses")]
    public partial class Networkingv1beta1Ingress : IKubernetesObject<V1ObjectMeta>, ISpec<Networkingv1beta1IngressSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "Ingress";
        public const string KubeGroup = "networking.k8s.io";
    }

    [KubernetesEntity(Group="networking.k8s.io", Kind="IngressClass", ApiVersion="v1beta1", PluralName="ingressclasses")]
    public partial class V1beta1IngressClass : IKubernetesObject<V1ObjectMeta>, ISpec<V1beta1IngressClassSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "IngressClass";
        public const string KubeGroup = "networking.k8s.io";
    }

    [KubernetesEntity(Group="networking.k8s.io", Kind="IngressClassList", ApiVersion="v1beta1", PluralName="ingressclasses")]
    public partial class V1beta1IngressClassList : IKubernetesObject<V1ListMeta>, IItems<V1beta1IngressClass>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "IngressClassList";
        public const string KubeGroup = "networking.k8s.io";
    }

    [KubernetesEntity(Group="networking.k8s.io", Kind="IngressList", ApiVersion="v1beta1", PluralName="ingresses")]
    public partial class Networkingv1beta1IngressList : IKubernetesObject<V1ListMeta>, IItems<Networkingv1beta1Ingress>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "IngressList";
        public const string KubeGroup = "networking.k8s.io";
    }

    [KubernetesEntity(Group="node.k8s.io", Kind="RuntimeClass", ApiVersion="v1alpha1", PluralName="runtimeclasses")]
    public partial class V1alpha1RuntimeClass : IKubernetesObject<V1ObjectMeta>, ISpec<V1alpha1RuntimeClassSpec>, IValidate
    {
        public const string KubeApiVersion = "v1alpha1";
        public const string KubeKind = "RuntimeClass";
        public const string KubeGroup = "node.k8s.io";
    }

    [KubernetesEntity(Group="node.k8s.io", Kind="RuntimeClassList", ApiVersion="v1alpha1", PluralName="runtimeclasses")]
    public partial class V1alpha1RuntimeClassList : IKubernetesObject<V1ListMeta>, IItems<V1alpha1RuntimeClass>, IValidate
    {
        public const string KubeApiVersion = "v1alpha1";
        public const string KubeKind = "RuntimeClassList";
        public const string KubeGroup = "node.k8s.io";
    }

    [KubernetesEntity(Group="node.k8s.io", Kind="RuntimeClass", ApiVersion="v1beta1", PluralName="runtimeclasses")]
    public partial class V1beta1RuntimeClass : IKubernetesObject<V1ObjectMeta>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "RuntimeClass";
        public const string KubeGroup = "node.k8s.io";
    }

    [KubernetesEntity(Group="node.k8s.io", Kind="RuntimeClassList", ApiVersion="v1beta1", PluralName="runtimeclasses")]
    public partial class V1beta1RuntimeClassList : IKubernetesObject<V1ListMeta>, IItems<V1beta1RuntimeClass>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "RuntimeClassList";
        public const string KubeGroup = "node.k8s.io";
    }

    [KubernetesEntity(Group="policy", Kind="Eviction", ApiVersion="v1beta1", PluralName=null)]
    public partial class V1beta1Eviction : IKubernetesObject<V1ObjectMeta>
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "Eviction";
        public const string KubeGroup = "policy";
    }

    [KubernetesEntity(Group="policy", Kind="PodDisruptionBudget", ApiVersion="v1beta1", PluralName="poddisruptionbudgets")]
    public partial class V1beta1PodDisruptionBudget : IKubernetesObject<V1ObjectMeta>, ISpec<V1beta1PodDisruptionBudgetSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "PodDisruptionBudget";
        public const string KubeGroup = "policy";
    }

    [KubernetesEntity(Group="policy", Kind="PodDisruptionBudgetList", ApiVersion="v1beta1", PluralName="poddisruptionbudgets")]
    public partial class V1beta1PodDisruptionBudgetList : IKubernetesObject<V1ListMeta>, IItems<V1beta1PodDisruptionBudget>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "PodDisruptionBudgetList";
        public const string KubeGroup = "policy";
    }

    [KubernetesEntity(Group="policy", Kind="PodSecurityPolicy", ApiVersion="v1beta1", PluralName="podsecuritypolicies")]
    public partial class V1beta1PodSecurityPolicy : IKubernetesObject<V1ObjectMeta>, ISpec<V1beta1PodSecurityPolicySpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "PodSecurityPolicy";
        public const string KubeGroup = "policy";
    }

    [KubernetesEntity(Group="policy", Kind="PodSecurityPolicyList", ApiVersion="v1beta1", PluralName="podsecuritypolicies")]
    public partial class V1beta1PodSecurityPolicyList : IKubernetesObject<V1ListMeta>, IItems<V1beta1PodSecurityPolicy>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "PodSecurityPolicyList";
        public const string KubeGroup = "policy";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="ClusterRole", ApiVersion="v1", PluralName="clusterroles")]
    public partial class V1ClusterRole : IKubernetesObject<V1ObjectMeta>
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "ClusterRole";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="ClusterRoleBinding", ApiVersion="v1", PluralName="clusterrolebindings")]
    public partial class V1ClusterRoleBinding : IKubernetesObject<V1ObjectMeta>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "ClusterRoleBinding";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="ClusterRoleBindingList", ApiVersion="v1", PluralName="clusterrolebindings")]
    public partial class V1ClusterRoleBindingList : IKubernetesObject<V1ListMeta>, IItems<V1ClusterRoleBinding>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "ClusterRoleBindingList";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="ClusterRoleList", ApiVersion="v1", PluralName="clusterroles")]
    public partial class V1ClusterRoleList : IKubernetesObject<V1ListMeta>, IItems<V1ClusterRole>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "ClusterRoleList";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="Role", ApiVersion="v1", PluralName="roles")]
    public partial class V1Role : IKubernetesObject<V1ObjectMeta>
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "Role";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="RoleBinding", ApiVersion="v1", PluralName="rolebindings")]
    public partial class V1RoleBinding : IKubernetesObject<V1ObjectMeta>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "RoleBinding";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="RoleBindingList", ApiVersion="v1", PluralName="rolebindings")]
    public partial class V1RoleBindingList : IKubernetesObject<V1ListMeta>, IItems<V1RoleBinding>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "RoleBindingList";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="RoleList", ApiVersion="v1", PluralName="roles")]
    public partial class V1RoleList : IKubernetesObject<V1ListMeta>, IItems<V1Role>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "RoleList";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="ClusterRole", ApiVersion="v1alpha1", PluralName="clusterroles")]
    public partial class V1alpha1ClusterRole : IKubernetesObject<V1ObjectMeta>
    {
        public const string KubeApiVersion = "v1alpha1";
        public const string KubeKind = "ClusterRole";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="ClusterRoleBinding", ApiVersion="v1alpha1", PluralName="clusterrolebindings")]
    public partial class V1alpha1ClusterRoleBinding : IKubernetesObject<V1ObjectMeta>, IValidate
    {
        public const string KubeApiVersion = "v1alpha1";
        public const string KubeKind = "ClusterRoleBinding";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="ClusterRoleBindingList", ApiVersion="v1alpha1", PluralName="clusterrolebindings")]
    public partial class V1alpha1ClusterRoleBindingList : IKubernetesObject<V1ListMeta>, IItems<V1alpha1ClusterRoleBinding>, IValidate
    {
        public const string KubeApiVersion = "v1alpha1";
        public const string KubeKind = "ClusterRoleBindingList";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="ClusterRoleList", ApiVersion="v1alpha1", PluralName="clusterroles")]
    public partial class V1alpha1ClusterRoleList : IKubernetesObject<V1ListMeta>, IItems<V1alpha1ClusterRole>, IValidate
    {
        public const string KubeApiVersion = "v1alpha1";
        public const string KubeKind = "ClusterRoleList";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="Role", ApiVersion="v1alpha1", PluralName="roles")]
    public partial class V1alpha1Role : IKubernetesObject<V1ObjectMeta>
    {
        public const string KubeApiVersion = "v1alpha1";
        public const string KubeKind = "Role";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="RoleBinding", ApiVersion="v1alpha1", PluralName="rolebindings")]
    public partial class V1alpha1RoleBinding : IKubernetesObject<V1ObjectMeta>, IValidate
    {
        public const string KubeApiVersion = "v1alpha1";
        public const string KubeKind = "RoleBinding";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="RoleBindingList", ApiVersion="v1alpha1", PluralName="rolebindings")]
    public partial class V1alpha1RoleBindingList : IKubernetesObject<V1ListMeta>, IItems<V1alpha1RoleBinding>, IValidate
    {
        public const string KubeApiVersion = "v1alpha1";
        public const string KubeKind = "RoleBindingList";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="RoleList", ApiVersion="v1alpha1", PluralName="roles")]
    public partial class V1alpha1RoleList : IKubernetesObject<V1ListMeta>, IItems<V1alpha1Role>, IValidate
    {
        public const string KubeApiVersion = "v1alpha1";
        public const string KubeKind = "RoleList";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="ClusterRole", ApiVersion="v1beta1", PluralName="clusterroles")]
    public partial class V1beta1ClusterRole : IKubernetesObject<V1ObjectMeta>
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "ClusterRole";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="ClusterRoleBinding", ApiVersion="v1beta1", PluralName="clusterrolebindings")]
    public partial class V1beta1ClusterRoleBinding : IKubernetesObject<V1ObjectMeta>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "ClusterRoleBinding";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="ClusterRoleBindingList", ApiVersion="v1beta1", PluralName="clusterrolebindings")]
    public partial class V1beta1ClusterRoleBindingList : IKubernetesObject<V1ListMeta>, IItems<V1beta1ClusterRoleBinding>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "ClusterRoleBindingList";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="ClusterRoleList", ApiVersion="v1beta1", PluralName="clusterroles")]
    public partial class V1beta1ClusterRoleList : IKubernetesObject<V1ListMeta>, IItems<V1beta1ClusterRole>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "ClusterRoleList";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="Role", ApiVersion="v1beta1", PluralName="roles")]
    public partial class V1beta1Role : IKubernetesObject<V1ObjectMeta>
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "Role";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="RoleBinding", ApiVersion="v1beta1", PluralName="rolebindings")]
    public partial class V1beta1RoleBinding : IKubernetesObject<V1ObjectMeta>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "RoleBinding";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="RoleBindingList", ApiVersion="v1beta1", PluralName="rolebindings")]
    public partial class V1beta1RoleBindingList : IKubernetesObject<V1ListMeta>, IItems<V1beta1RoleBinding>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "RoleBindingList";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="rbac.authorization.k8s.io", Kind="RoleList", ApiVersion="v1beta1", PluralName="roles")]
    public partial class V1beta1RoleList : IKubernetesObject<V1ListMeta>, IItems<V1beta1Role>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "RoleList";
        public const string KubeGroup = "rbac.authorization.k8s.io";
    }

    [KubernetesEntity(Group="scheduling.k8s.io", Kind="PriorityClass", ApiVersion="v1", PluralName="priorityclasses")]
    public partial class V1PriorityClass : IKubernetesObject<V1ObjectMeta>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "PriorityClass";
        public const string KubeGroup = "scheduling.k8s.io";
    }

    [KubernetesEntity(Group="scheduling.k8s.io", Kind="PriorityClassList", ApiVersion="v1", PluralName="priorityclasses")]
    public partial class V1PriorityClassList : IKubernetesObject<V1ListMeta>, IItems<V1PriorityClass>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "PriorityClassList";
        public const string KubeGroup = "scheduling.k8s.io";
    }

    [KubernetesEntity(Group="scheduling.k8s.io", Kind="PriorityClass", ApiVersion="v1alpha1", PluralName="priorityclasses")]
    public partial class V1alpha1PriorityClass : IKubernetesObject<V1ObjectMeta>, IValidate
    {
        public const string KubeApiVersion = "v1alpha1";
        public const string KubeKind = "PriorityClass";
        public const string KubeGroup = "scheduling.k8s.io";
    }

    [KubernetesEntity(Group="scheduling.k8s.io", Kind="PriorityClassList", ApiVersion="v1alpha1", PluralName="priorityclasses")]
    public partial class V1alpha1PriorityClassList : IKubernetesObject<V1ListMeta>, IItems<V1alpha1PriorityClass>, IValidate
    {
        public const string KubeApiVersion = "v1alpha1";
        public const string KubeKind = "PriorityClassList";
        public const string KubeGroup = "scheduling.k8s.io";
    }

    [KubernetesEntity(Group="scheduling.k8s.io", Kind="PriorityClass", ApiVersion="v1beta1", PluralName="priorityclasses")]
    public partial class V1beta1PriorityClass : IKubernetesObject<V1ObjectMeta>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "PriorityClass";
        public const string KubeGroup = "scheduling.k8s.io";
    }

    [KubernetesEntity(Group="scheduling.k8s.io", Kind="PriorityClassList", ApiVersion="v1beta1", PluralName="priorityclasses")]
    public partial class V1beta1PriorityClassList : IKubernetesObject<V1ListMeta>, IItems<V1beta1PriorityClass>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "PriorityClassList";
        public const string KubeGroup = "scheduling.k8s.io";
    }

    [KubernetesEntity(Group="settings.k8s.io", Kind="PodPreset", ApiVersion="v1alpha1", PluralName="podpresets")]
    public partial class V1alpha1PodPreset : IKubernetesObject<V1ObjectMeta>, ISpec<V1alpha1PodPresetSpec>
    {
        public const string KubeApiVersion = "v1alpha1";
        public const string KubeKind = "PodPreset";
        public const string KubeGroup = "settings.k8s.io";
    }

    [KubernetesEntity(Group="settings.k8s.io", Kind="PodPresetList", ApiVersion="v1alpha1", PluralName="podpresets")]
    public partial class V1alpha1PodPresetList : IKubernetesObject<V1ListMeta>, IItems<V1alpha1PodPreset>, IValidate
    {
        public const string KubeApiVersion = "v1alpha1";
        public const string KubeKind = "PodPresetList";
        public const string KubeGroup = "settings.k8s.io";
    }

    [KubernetesEntity(Group="storage.k8s.io", Kind="CSIDriver", ApiVersion="v1", PluralName="csidrivers")]
    public partial class V1CSIDriver : IKubernetesObject<V1ObjectMeta>, ISpec<V1CSIDriverSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "CSIDriver";
        public const string KubeGroup = "storage.k8s.io";
    }

    [KubernetesEntity(Group="storage.k8s.io", Kind="CSIDriverList", ApiVersion="v1", PluralName="csidrivers")]
    public partial class V1CSIDriverList : IKubernetesObject<V1ListMeta>, IItems<V1CSIDriver>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "CSIDriverList";
        public const string KubeGroup = "storage.k8s.io";
    }

    [KubernetesEntity(Group="storage.k8s.io", Kind="CSINode", ApiVersion="v1", PluralName="csinodes")]
    public partial class V1CSINode : IKubernetesObject<V1ObjectMeta>, ISpec<V1CSINodeSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "CSINode";
        public const string KubeGroup = "storage.k8s.io";
    }

    [KubernetesEntity(Group="storage.k8s.io", Kind="CSINodeList", ApiVersion="v1", PluralName="csinodes")]
    public partial class V1CSINodeList : IKubernetesObject<V1ListMeta>, IItems<V1CSINode>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "CSINodeList";
        public const string KubeGroup = "storage.k8s.io";
    }

    [KubernetesEntity(Group="storage.k8s.io", Kind="StorageClass", ApiVersion="v1", PluralName="storageclasses")]
    public partial class V1StorageClass : IKubernetesObject<V1ObjectMeta>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "StorageClass";
        public const string KubeGroup = "storage.k8s.io";
    }

    [KubernetesEntity(Group="storage.k8s.io", Kind="StorageClassList", ApiVersion="v1", PluralName="storageclasses")]
    public partial class V1StorageClassList : IKubernetesObject<V1ListMeta>, IItems<V1StorageClass>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "StorageClassList";
        public const string KubeGroup = "storage.k8s.io";
    }

    [KubernetesEntity(Group="storage.k8s.io", Kind="VolumeAttachment", ApiVersion="v1", PluralName="volumeattachments")]
    public partial class V1VolumeAttachment : IKubernetesObject<V1ObjectMeta>, ISpec<V1VolumeAttachmentSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "VolumeAttachment";
        public const string KubeGroup = "storage.k8s.io";
    }

    [KubernetesEntity(Group="storage.k8s.io", Kind="VolumeAttachmentList", ApiVersion="v1", PluralName="volumeattachments")]
    public partial class V1VolumeAttachmentList : IKubernetesObject<V1ListMeta>, IItems<V1VolumeAttachment>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "VolumeAttachmentList";
        public const string KubeGroup = "storage.k8s.io";
    }

    [KubernetesEntity(Group="storage.k8s.io", Kind="VolumeAttachment", ApiVersion="v1alpha1", PluralName="volumeattachments")]
    public partial class V1alpha1VolumeAttachment : IKubernetesObject<V1ObjectMeta>, ISpec<V1alpha1VolumeAttachmentSpec>, IValidate
    {
        public const string KubeApiVersion = "v1alpha1";
        public const string KubeKind = "VolumeAttachment";
        public const string KubeGroup = "storage.k8s.io";
    }

    [KubernetesEntity(Group="storage.k8s.io", Kind="VolumeAttachmentList", ApiVersion="v1alpha1", PluralName="volumeattachments")]
    public partial class V1alpha1VolumeAttachmentList : IKubernetesObject<V1ListMeta>, IItems<V1alpha1VolumeAttachment>, IValidate
    {
        public const string KubeApiVersion = "v1alpha1";
        public const string KubeKind = "VolumeAttachmentList";
        public const string KubeGroup = "storage.k8s.io";
    }

    [KubernetesEntity(Group="storage.k8s.io", Kind="CSIDriver", ApiVersion="v1beta1", PluralName="csidrivers")]
    public partial class V1beta1CSIDriver : IKubernetesObject<V1ObjectMeta>, ISpec<V1beta1CSIDriverSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "CSIDriver";
        public const string KubeGroup = "storage.k8s.io";
    }

    [KubernetesEntity(Group="storage.k8s.io", Kind="CSIDriverList", ApiVersion="v1beta1", PluralName="csidrivers")]
    public partial class V1beta1CSIDriverList : IKubernetesObject<V1ListMeta>, IItems<V1beta1CSIDriver>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "CSIDriverList";
        public const string KubeGroup = "storage.k8s.io";
    }

    [KubernetesEntity(Group="storage.k8s.io", Kind="CSINode", ApiVersion="v1beta1", PluralName="csinodes")]
    public partial class V1beta1CSINode : IKubernetesObject<V1ObjectMeta>, ISpec<V1beta1CSINodeSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "CSINode";
        public const string KubeGroup = "storage.k8s.io";
    }

    [KubernetesEntity(Group="storage.k8s.io", Kind="CSINodeList", ApiVersion="v1beta1", PluralName="csinodes")]
    public partial class V1beta1CSINodeList : IKubernetesObject<V1ListMeta>, IItems<V1beta1CSINode>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "CSINodeList";
        public const string KubeGroup = "storage.k8s.io";
    }

    [KubernetesEntity(Group="storage.k8s.io", Kind="StorageClass", ApiVersion="v1beta1", PluralName="storageclasses")]
    public partial class V1beta1StorageClass : IKubernetesObject<V1ObjectMeta>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "StorageClass";
        public const string KubeGroup = "storage.k8s.io";
    }

    [KubernetesEntity(Group="storage.k8s.io", Kind="StorageClassList", ApiVersion="v1beta1", PluralName="storageclasses")]
    public partial class V1beta1StorageClassList : IKubernetesObject<V1ListMeta>, IItems<V1beta1StorageClass>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "StorageClassList";
        public const string KubeGroup = "storage.k8s.io";
    }

    [KubernetesEntity(Group="storage.k8s.io", Kind="VolumeAttachment", ApiVersion="v1beta1", PluralName="volumeattachments")]
    public partial class V1beta1VolumeAttachment : IKubernetesObject<V1ObjectMeta>, ISpec<V1beta1VolumeAttachmentSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "VolumeAttachment";
        public const string KubeGroup = "storage.k8s.io";
    }

    [KubernetesEntity(Group="storage.k8s.io", Kind="VolumeAttachmentList", ApiVersion="v1beta1", PluralName="volumeattachments")]
    public partial class V1beta1VolumeAttachmentList : IKubernetesObject<V1ListMeta>, IItems<V1beta1VolumeAttachment>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "VolumeAttachmentList";
        public const string KubeGroup = "storage.k8s.io";
    }

    [KubernetesEntity(Group="apiextensions.k8s.io", Kind="CustomResourceDefinition", ApiVersion="v1", PluralName="customresourcedefinitions")]
    public partial class V1CustomResourceDefinition : IKubernetesObject<V1ObjectMeta>, ISpec<V1CustomResourceDefinitionSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "CustomResourceDefinition";
        public const string KubeGroup = "apiextensions.k8s.io";
    }

    [KubernetesEntity(Group="apiextensions.k8s.io", Kind="CustomResourceDefinitionList", ApiVersion="v1", PluralName="customresourcedefinitions")]
    public partial class V1CustomResourceDefinitionList : IKubernetesObject<V1ListMeta>, IItems<V1CustomResourceDefinition>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "CustomResourceDefinitionList";
        public const string KubeGroup = "apiextensions.k8s.io";
    }

    [KubernetesEntity(Group="apiextensions.k8s.io", Kind="CustomResourceDefinition", ApiVersion="v1beta1", PluralName="customresourcedefinitions")]
    public partial class V1beta1CustomResourceDefinition : IKubernetesObject<V1ObjectMeta>, ISpec<V1beta1CustomResourceDefinitionSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "CustomResourceDefinition";
        public const string KubeGroup = "apiextensions.k8s.io";
    }

    [KubernetesEntity(Group="apiextensions.k8s.io", Kind="CustomResourceDefinitionList", ApiVersion="v1beta1", PluralName="customresourcedefinitions")]
    public partial class V1beta1CustomResourceDefinitionList : IKubernetesObject<V1ListMeta>, IItems<V1beta1CustomResourceDefinition>, IValidate
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
    public partial class V1Status : IKubernetesObject<V1ListMeta>
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "Status";
        public const string KubeGroup = "";
    }

    [KubernetesEntity(Group="apiregistration.k8s.io", Kind="APIService", ApiVersion="v1", PluralName="apiservices")]
    public partial class V1APIService : IKubernetesObject<V1ObjectMeta>, ISpec<V1APIServiceSpec>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "APIService";
        public const string KubeGroup = "apiregistration.k8s.io";
    }

    [KubernetesEntity(Group="apiregistration.k8s.io", Kind="APIServiceList", ApiVersion="v1", PluralName="apiservices")]
    public partial class V1APIServiceList : IKubernetesObject<V1ListMeta>, IItems<V1APIService>, IValidate
    {
        public const string KubeApiVersion = "v1";
        public const string KubeKind = "APIServiceList";
        public const string KubeGroup = "apiregistration.k8s.io";
    }

    [KubernetesEntity(Group="apiregistration.k8s.io", Kind="APIService", ApiVersion="v1beta1", PluralName="apiservices")]
    public partial class V1beta1APIService : IKubernetesObject<V1ObjectMeta>, ISpec<V1beta1APIServiceSpec>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "APIService";
        public const string KubeGroup = "apiregistration.k8s.io";
    }

    [KubernetesEntity(Group="apiregistration.k8s.io", Kind="APIServiceList", ApiVersion="v1beta1", PluralName="apiservices")]
    public partial class V1beta1APIServiceList : IKubernetesObject<V1ListMeta>, IItems<V1beta1APIService>, IValidate
    {
        public const string KubeApiVersion = "v1beta1";
        public const string KubeKind = "APIServiceList";
        public const string KubeGroup = "apiregistration.k8s.io";
    }

}
