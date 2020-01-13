namespace k8s.Models
{
    public partial class V1MutatingWebhookConfiguration : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "MutatingWebhookConfiguration";
        private const string kubeGroup = "admissionregistration.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1MutatingWebhookConfigurationList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "MutatingWebhookConfigurationList";
        private const string kubeGroup = "admissionregistration.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1ValidatingWebhookConfiguration : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "ValidatingWebhookConfiguration";
        private const string kubeGroup = "admissionregistration.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1ValidatingWebhookConfigurationList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "ValidatingWebhookConfigurationList";
        private const string kubeGroup = "admissionregistration.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1MutatingWebhookConfiguration : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "MutatingWebhookConfiguration";
        private const string kubeGroup = "admissionregistration.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1MutatingWebhookConfigurationList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "MutatingWebhookConfigurationList";
        private const string kubeGroup = "admissionregistration.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1ValidatingWebhookConfiguration : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "ValidatingWebhookConfiguration";
        private const string kubeGroup = "admissionregistration.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1ValidatingWebhookConfigurationList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "ValidatingWebhookConfigurationList";
        private const string kubeGroup = "admissionregistration.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1ControllerRevision : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "ControllerRevision";
        private const string kubeGroup = "apps";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1ControllerRevisionList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "ControllerRevisionList";
        private const string kubeGroup = "apps";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1DaemonSet : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "DaemonSet";
        private const string kubeGroup = "apps";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1DaemonSetList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "DaemonSetList";
        private const string kubeGroup = "apps";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1Deployment : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "Deployment";
        private const string kubeGroup = "apps";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1DeploymentList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "DeploymentList";
        private const string kubeGroup = "apps";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1ReplicaSet : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "ReplicaSet";
        private const string kubeGroup = "apps";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1ReplicaSetList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "ReplicaSetList";
        private const string kubeGroup = "apps";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1StatefulSet : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "StatefulSet";
        private const string kubeGroup = "apps";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1StatefulSetList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "StatefulSetList";
        private const string kubeGroup = "apps";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1ControllerRevision : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "ControllerRevision";
        private const string kubeGroup = "apps";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1ControllerRevisionList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "ControllerRevisionList";
        private const string kubeGroup = "apps";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1StatefulSet : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "StatefulSet";
        private const string kubeGroup = "apps";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1StatefulSetList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "StatefulSetList";
        private const string kubeGroup = "apps";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta2ControllerRevision : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta2";	
        private const string kubeKind = "ControllerRevision";
        private const string kubeGroup = "apps";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta2ControllerRevisionList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta2";	
        private const string kubeKind = "ControllerRevisionList";
        private const string kubeGroup = "apps";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta2DaemonSet : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta2";	
        private const string kubeKind = "DaemonSet";
        private const string kubeGroup = "apps";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta2DaemonSetList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta2";	
        private const string kubeKind = "DaemonSetList";
        private const string kubeGroup = "apps";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta2Deployment : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta2";	
        private const string kubeKind = "Deployment";
        private const string kubeGroup = "apps";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta2DeploymentList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta2";	
        private const string kubeKind = "DeploymentList";
        private const string kubeGroup = "apps";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta2ReplicaSet : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta2";	
        private const string kubeKind = "ReplicaSet";
        private const string kubeGroup = "apps";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta2ReplicaSetList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta2";	
        private const string kubeKind = "ReplicaSetList";
        private const string kubeGroup = "apps";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta2Scale : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta2";	
        private const string kubeKind = "Scale";
        private const string kubeGroup = "apps";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta2StatefulSet : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta2";	
        private const string kubeKind = "StatefulSet";
        private const string kubeGroup = "apps";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta2StatefulSetList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta2";	
        private const string kubeKind = "StatefulSetList";
        private const string kubeGroup = "apps";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1alpha1AuditSink : IKubernetesObject
    {
        private const string kubeApiVersion = "v1alpha1";	
        private const string kubeKind = "AuditSink";
        private const string kubeGroup = "auditregistration.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1alpha1AuditSinkList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1alpha1";	
        private const string kubeKind = "AuditSinkList";
        private const string kubeGroup = "auditregistration.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1TokenRequest : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "TokenRequest";
        private const string kubeGroup = "authentication.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1TokenReview : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "TokenReview";
        private const string kubeGroup = "authentication.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1TokenReview : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "TokenReview";
        private const string kubeGroup = "authentication.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1LocalSubjectAccessReview : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "LocalSubjectAccessReview";
        private const string kubeGroup = "authorization.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1SelfSubjectAccessReview : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "SelfSubjectAccessReview";
        private const string kubeGroup = "authorization.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1SelfSubjectRulesReview : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "SelfSubjectRulesReview";
        private const string kubeGroup = "authorization.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1SubjectAccessReview : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "SubjectAccessReview";
        private const string kubeGroup = "authorization.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1LocalSubjectAccessReview : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "LocalSubjectAccessReview";
        private const string kubeGroup = "authorization.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1SelfSubjectAccessReview : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "SelfSubjectAccessReview";
        private const string kubeGroup = "authorization.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1SelfSubjectRulesReview : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "SelfSubjectRulesReview";
        private const string kubeGroup = "authorization.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1SubjectAccessReview : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "SubjectAccessReview";
        private const string kubeGroup = "authorization.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1HorizontalPodAutoscaler : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "HorizontalPodAutoscaler";
        private const string kubeGroup = "autoscaling";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1HorizontalPodAutoscalerList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "HorizontalPodAutoscalerList";
        private const string kubeGroup = "autoscaling";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1Scale : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "Scale";
        private const string kubeGroup = "autoscaling";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V2beta1HorizontalPodAutoscaler : IKubernetesObject
    {
        private const string kubeApiVersion = "v2beta1";	
        private const string kubeKind = "HorizontalPodAutoscaler";
        private const string kubeGroup = "autoscaling";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V2beta1HorizontalPodAutoscalerList : IKubernetesObject
    {
        private const string kubeApiVersion = "v2beta1";	
        private const string kubeKind = "HorizontalPodAutoscalerList";
        private const string kubeGroup = "autoscaling";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V2beta2HorizontalPodAutoscaler : IKubernetesObject
    {
        private const string kubeApiVersion = "v2beta2";	
        private const string kubeKind = "HorizontalPodAutoscaler";
        private const string kubeGroup = "autoscaling";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V2beta2HorizontalPodAutoscalerList : IKubernetesObject
    {
        private const string kubeApiVersion = "v2beta2";	
        private const string kubeKind = "HorizontalPodAutoscalerList";
        private const string kubeGroup = "autoscaling";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1Job : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "Job";
        private const string kubeGroup = "batch";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1JobList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "JobList";
        private const string kubeGroup = "batch";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1CronJob : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "CronJob";
        private const string kubeGroup = "batch";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1CronJobList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "CronJobList";
        private const string kubeGroup = "batch";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V2alpha1CronJob : IKubernetesObject
    {
        private const string kubeApiVersion = "v2alpha1";	
        private const string kubeKind = "CronJob";
        private const string kubeGroup = "batch";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V2alpha1CronJobList : IKubernetesObject
    {
        private const string kubeApiVersion = "v2alpha1";	
        private const string kubeKind = "CronJobList";
        private const string kubeGroup = "batch";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1CertificateSigningRequest : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "CertificateSigningRequest";
        private const string kubeGroup = "certificates.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1CertificateSigningRequestList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "CertificateSigningRequestList";
        private const string kubeGroup = "certificates.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1Lease : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "Lease";
        private const string kubeGroup = "coordination.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1LeaseList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "LeaseList";
        private const string kubeGroup = "coordination.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1Lease : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "Lease";
        private const string kubeGroup = "coordination.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1LeaseList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "LeaseList";
        private const string kubeGroup = "coordination.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1Binding : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "Binding";
        private const string kubeGroup = "";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1ComponentStatus : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "ComponentStatus";
        private const string kubeGroup = "";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1ComponentStatusList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "ComponentStatusList";
        private const string kubeGroup = "";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1ConfigMap : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "ConfigMap";
        private const string kubeGroup = "";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1ConfigMapList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "ConfigMapList";
        private const string kubeGroup = "";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1Endpoints : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "Endpoints";
        private const string kubeGroup = "";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1EndpointsList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "EndpointsList";
        private const string kubeGroup = "";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1Event : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "Event";
        private const string kubeGroup = "";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1EventList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "EventList";
        private const string kubeGroup = "";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1LimitRange : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "LimitRange";
        private const string kubeGroup = "";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1LimitRangeList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "LimitRangeList";
        private const string kubeGroup = "";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1Namespace : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "Namespace";
        private const string kubeGroup = "";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1NamespaceList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "NamespaceList";
        private const string kubeGroup = "";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1Node : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "Node";
        private const string kubeGroup = "";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1NodeList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "NodeList";
        private const string kubeGroup = "";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1PersistentVolume : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "PersistentVolume";
        private const string kubeGroup = "";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1PersistentVolumeClaim : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "PersistentVolumeClaim";
        private const string kubeGroup = "";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1PersistentVolumeClaimList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "PersistentVolumeClaimList";
        private const string kubeGroup = "";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1PersistentVolumeList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "PersistentVolumeList";
        private const string kubeGroup = "";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1Pod : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "Pod";
        private const string kubeGroup = "";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1PodList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "PodList";
        private const string kubeGroup = "";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1PodTemplate : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "PodTemplate";
        private const string kubeGroup = "";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1PodTemplateList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "PodTemplateList";
        private const string kubeGroup = "";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1ReplicationController : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "ReplicationController";
        private const string kubeGroup = "";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1ReplicationControllerList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "ReplicationControllerList";
        private const string kubeGroup = "";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1ResourceQuota : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "ResourceQuota";
        private const string kubeGroup = "";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1ResourceQuotaList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "ResourceQuotaList";
        private const string kubeGroup = "";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1Secret : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "Secret";
        private const string kubeGroup = "";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1SecretList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "SecretList";
        private const string kubeGroup = "";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1Service : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "Service";
        private const string kubeGroup = "";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1ServiceAccount : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "ServiceAccount";
        private const string kubeGroup = "";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1ServiceAccountList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "ServiceAccountList";
        private const string kubeGroup = "";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1ServiceList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "ServiceList";
        private const string kubeGroup = "";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1alpha1EndpointSlice : IKubernetesObject
    {
        private const string kubeApiVersion = "v1alpha1";	
        private const string kubeKind = "EndpointSlice";
        private const string kubeGroup = "discovery.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1alpha1EndpointSliceList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1alpha1";	
        private const string kubeKind = "EndpointSliceList";
        private const string kubeGroup = "discovery.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1Event : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "Event";
        private const string kubeGroup = "events.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1EventList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "EventList";
        private const string kubeGroup = "events.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1DaemonSet : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "DaemonSet";
        private const string kubeGroup = "extensions";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1DaemonSetList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "DaemonSetList";
        private const string kubeGroup = "extensions";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1NetworkPolicy : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "NetworkPolicy";
        private const string kubeGroup = "extensions";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1NetworkPolicyList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "NetworkPolicyList";
        private const string kubeGroup = "extensions";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1ReplicaSet : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "ReplicaSet";
        private const string kubeGroup = "extensions";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1ReplicaSetList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "ReplicaSetList";
        private const string kubeGroup = "extensions";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1NetworkPolicy : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "NetworkPolicy";
        private const string kubeGroup = "networking.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1NetworkPolicyList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "NetworkPolicyList";
        private const string kubeGroup = "networking.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1alpha1RuntimeClass : IKubernetesObject
    {
        private const string kubeApiVersion = "v1alpha1";	
        private const string kubeKind = "RuntimeClass";
        private const string kubeGroup = "node.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1alpha1RuntimeClassList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1alpha1";	
        private const string kubeKind = "RuntimeClassList";
        private const string kubeGroup = "node.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1RuntimeClass : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "RuntimeClass";
        private const string kubeGroup = "node.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1RuntimeClassList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "RuntimeClassList";
        private const string kubeGroup = "node.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1Eviction : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "Eviction";
        private const string kubeGroup = "policy";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1PodDisruptionBudget : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "PodDisruptionBudget";
        private const string kubeGroup = "policy";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1PodDisruptionBudgetList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "PodDisruptionBudgetList";
        private const string kubeGroup = "policy";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1ClusterRole : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "ClusterRole";
        private const string kubeGroup = "rbac.authorization.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1ClusterRoleBinding : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "ClusterRoleBinding";
        private const string kubeGroup = "rbac.authorization.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1ClusterRoleBindingList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "ClusterRoleBindingList";
        private const string kubeGroup = "rbac.authorization.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1ClusterRoleList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "ClusterRoleList";
        private const string kubeGroup = "rbac.authorization.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1Role : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "Role";
        private const string kubeGroup = "rbac.authorization.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1RoleBinding : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "RoleBinding";
        private const string kubeGroup = "rbac.authorization.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1RoleBindingList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "RoleBindingList";
        private const string kubeGroup = "rbac.authorization.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1RoleList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "RoleList";
        private const string kubeGroup = "rbac.authorization.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1alpha1ClusterRole : IKubernetesObject
    {
        private const string kubeApiVersion = "v1alpha1";	
        private const string kubeKind = "ClusterRole";
        private const string kubeGroup = "rbac.authorization.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1alpha1ClusterRoleBinding : IKubernetesObject
    {
        private const string kubeApiVersion = "v1alpha1";	
        private const string kubeKind = "ClusterRoleBinding";
        private const string kubeGroup = "rbac.authorization.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1alpha1ClusterRoleBindingList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1alpha1";	
        private const string kubeKind = "ClusterRoleBindingList";
        private const string kubeGroup = "rbac.authorization.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1alpha1ClusterRoleList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1alpha1";	
        private const string kubeKind = "ClusterRoleList";
        private const string kubeGroup = "rbac.authorization.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1alpha1Role : IKubernetesObject
    {
        private const string kubeApiVersion = "v1alpha1";	
        private const string kubeKind = "Role";
        private const string kubeGroup = "rbac.authorization.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1alpha1RoleBinding : IKubernetesObject
    {
        private const string kubeApiVersion = "v1alpha1";	
        private const string kubeKind = "RoleBinding";
        private const string kubeGroup = "rbac.authorization.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1alpha1RoleBindingList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1alpha1";	
        private const string kubeKind = "RoleBindingList";
        private const string kubeGroup = "rbac.authorization.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1alpha1RoleList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1alpha1";	
        private const string kubeKind = "RoleList";
        private const string kubeGroup = "rbac.authorization.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1ClusterRole : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "ClusterRole";
        private const string kubeGroup = "rbac.authorization.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1ClusterRoleBinding : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "ClusterRoleBinding";
        private const string kubeGroup = "rbac.authorization.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1ClusterRoleBindingList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "ClusterRoleBindingList";
        private const string kubeGroup = "rbac.authorization.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1ClusterRoleList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "ClusterRoleList";
        private const string kubeGroup = "rbac.authorization.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1Role : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "Role";
        private const string kubeGroup = "rbac.authorization.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1RoleBinding : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "RoleBinding";
        private const string kubeGroup = "rbac.authorization.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1RoleBindingList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "RoleBindingList";
        private const string kubeGroup = "rbac.authorization.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1RoleList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "RoleList";
        private const string kubeGroup = "rbac.authorization.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1PriorityClass : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "PriorityClass";
        private const string kubeGroup = "scheduling.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1PriorityClassList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "PriorityClassList";
        private const string kubeGroup = "scheduling.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1alpha1PriorityClass : IKubernetesObject
    {
        private const string kubeApiVersion = "v1alpha1";	
        private const string kubeKind = "PriorityClass";
        private const string kubeGroup = "scheduling.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1alpha1PriorityClassList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1alpha1";	
        private const string kubeKind = "PriorityClassList";
        private const string kubeGroup = "scheduling.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1PriorityClass : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "PriorityClass";
        private const string kubeGroup = "scheduling.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1PriorityClassList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "PriorityClassList";
        private const string kubeGroup = "scheduling.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1alpha1PodPreset : IKubernetesObject
    {
        private const string kubeApiVersion = "v1alpha1";	
        private const string kubeKind = "PodPreset";
        private const string kubeGroup = "settings.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1alpha1PodPresetList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1alpha1";	
        private const string kubeKind = "PodPresetList";
        private const string kubeGroup = "settings.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1StorageClass : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "StorageClass";
        private const string kubeGroup = "storage.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1StorageClassList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "StorageClassList";
        private const string kubeGroup = "storage.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1VolumeAttachment : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "VolumeAttachment";
        private const string kubeGroup = "storage.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1VolumeAttachmentList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "VolumeAttachmentList";
        private const string kubeGroup = "storage.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1alpha1VolumeAttachment : IKubernetesObject
    {
        private const string kubeApiVersion = "v1alpha1";	
        private const string kubeKind = "VolumeAttachment";
        private const string kubeGroup = "storage.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1alpha1VolumeAttachmentList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1alpha1";	
        private const string kubeKind = "VolumeAttachmentList";
        private const string kubeGroup = "storage.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1CSIDriver : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "CSIDriver";
        private const string kubeGroup = "storage.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1CSIDriverList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "CSIDriverList";
        private const string kubeGroup = "storage.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1CSINode : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "CSINode";
        private const string kubeGroup = "storage.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1CSINodeList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "CSINodeList";
        private const string kubeGroup = "storage.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1StorageClass : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "StorageClass";
        private const string kubeGroup = "storage.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1StorageClassList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "StorageClassList";
        private const string kubeGroup = "storage.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1VolumeAttachment : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "VolumeAttachment";
        private const string kubeGroup = "storage.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1VolumeAttachmentList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "VolumeAttachmentList";
        private const string kubeGroup = "storage.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1CustomResourceDefinition : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "CustomResourceDefinition";
        private const string kubeGroup = "apiextensions.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1CustomResourceDefinitionList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "CustomResourceDefinitionList";
        private const string kubeGroup = "apiextensions.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1CustomResourceDefinition : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "CustomResourceDefinition";
        private const string kubeGroup = "apiextensions.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1CustomResourceDefinitionList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "CustomResourceDefinitionList";
        private const string kubeGroup = "apiextensions.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1APIGroup : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "APIGroup";
        private const string kubeGroup = "";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1APIGroupList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "APIGroupList";
        private const string kubeGroup = "";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1APIResourceList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "APIResourceList";
        private const string kubeGroup = "";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1APIVersions : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "APIVersions";
        private const string kubeGroup = "";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1DeleteOptions : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "DeleteOptions";
        private const string kubeGroup = "";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1Status : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "Status";
        private const string kubeGroup = "";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1APIService : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "APIService";
        private const string kubeGroup = "apiregistration.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1APIServiceList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1";	
        private const string kubeKind = "APIServiceList";
        private const string kubeGroup = "apiregistration.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1APIService : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "APIService";
        private const string kubeGroup = "apiregistration.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

    public partial class V1beta1APIServiceList : IKubernetesObject
    {
        private const string kubeApiVersion = "v1beta1";	
        private const string kubeKind = "APIServiceList";
        private const string kubeGroup = "apiregistration.k8s.io";	
        public string KubeApiVersion { get { return kubeApiVersion;} }		
        public string KubeKind {get {return kubeKind;}}		
        public string KubeGroup {get {return kubeGroup;}}		     
    }

}
