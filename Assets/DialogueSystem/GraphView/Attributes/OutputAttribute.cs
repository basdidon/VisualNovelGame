using System;

namespace BasDidon.Dialogue.VisualGraphView
{
    [AttributeUsage(AttributeTargets.Property)]
    public class OutputAttribute : PortAttribute
    {
        public OutputAttribute(string backingFieldName = null) : base(backingFieldName) { }
    }
}
