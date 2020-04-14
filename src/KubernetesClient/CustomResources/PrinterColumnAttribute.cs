using System;

namespace k8s.CustomResources
{
    public class PrinterColumnAttribute : Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; } = -1;

    }
}
