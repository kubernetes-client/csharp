namespace k8s.Controllers.Reconciler
{
    public class Request
    {
        protected bool Equals(Request other)
        {
            return Name == other.Name && Namespace == other.Namespace;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Request) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (Namespace != null ? Namespace.GetHashCode() : 0);
            }
        }

        public Request(string name)
        {
            Name = name;
        }

        public Request(string @namespace, string name)
        {
            Name = name;
            Namespace = @namespace;
        }

        public string Name { get; set; }
        public string Namespace { get; set; }

        public override string ToString() => Namespace == null ? Name : $"{Namespace}/{Name}";
    }
}