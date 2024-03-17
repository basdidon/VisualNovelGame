using System;

namespace BasDidon.Dialogue.VisualGraphView
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