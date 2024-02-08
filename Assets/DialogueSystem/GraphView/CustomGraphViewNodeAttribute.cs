using System;

namespace BasDidon.Dialogue.VisualGraphView
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CustomGraphViewNodeAttribute : Attribute
    {
        public Type Type;
        public CustomGraphViewNodeAttribute(Type type)
        {
            Type = type;
        }
    }
}