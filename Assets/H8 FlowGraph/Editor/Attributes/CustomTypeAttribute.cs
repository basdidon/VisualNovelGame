using System;

namespace H8.FlowGraph.UiElements
{

    [AttributeUsage(AttributeTargets.Class)]
    public class CustomTypeAttribute : Attribute {
        public IPortFactory PortFactoryType { get; }
        public CustomTypeAttribute(IPortFactory portFactoryType)
        {
            PortFactoryType = portFactoryType;
        }
    }
}