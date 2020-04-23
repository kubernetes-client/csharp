namespace k8s
{
    /// <summary>
    /// Object that allows self validation
    /// </summary>
    public interface IValidate
    {
        /// <summary>
        /// Validate the object.
        /// </summary>
        void Validate();
    }
}
