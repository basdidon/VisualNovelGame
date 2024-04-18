using System;

namespace H8.FlowGraph
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
