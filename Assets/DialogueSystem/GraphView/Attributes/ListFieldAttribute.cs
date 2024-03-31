using System;

namespace H8.GraphView
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ListFieldAttribute : Attribute
    {
        public Type CreatorType { get; }
        public ListFieldAttribute(Type type)
        {
            CreatorType = type;
        }
    }
}
