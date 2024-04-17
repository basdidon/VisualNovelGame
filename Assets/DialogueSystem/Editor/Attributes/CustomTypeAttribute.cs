using System;

namespace H8.GraphView.UiElements
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