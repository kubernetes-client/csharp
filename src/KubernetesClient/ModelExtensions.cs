using System.Net;

namespace k8s.Models
{
    public partial class V1Status
    {
        /// <summary>Converts a <see cref="V1Status"/> object into a short description of the status.</summary>
        public override string ToString()
        {
            string reason = Reason;
            if (string.IsNullOrEmpty(reason) && Code.GetValueOrDefault() != 0)
            {
                reason = ((HttpStatusCode)Code.Value).ToString();
            }
            return string.IsNullOrEmpty(Message) ? string.IsNullOrEmpty(reason) ? Status : reason :
                   string.IsNullOrEmpty(reason) ? Message : $"{reason} - {Message}";
        }
    }
}
