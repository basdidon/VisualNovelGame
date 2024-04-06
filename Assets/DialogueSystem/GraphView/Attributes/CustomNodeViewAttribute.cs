using System;
using System.Linq;
using System.Reflection;

namespace H8.GraphView.UiElements
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CustomNodeViewAttribute : Attribute
    {
        public Type Type { get; }
        public CustomNodeViewAttribute(Type type)
        {
            Type = type;
        }

        public static Type GetGraphViewNodeType(Type type)  
        {
            var allTypes = Assembly.GetExecutingAssembly().GetTypes();
            var typesWithAttribute = allTypes
                .Where(t => t.IsSubclassOf(typeof(NodeView)) && IsDefined(t, typeof(CustomNodeViewAttribute)));

            var nodeTypeAttribute = typesWithAttribute
                .Select(t => new { Type = t, Attribute = (CustomNodeViewAttribute) GetCustomAttribute(t, typeof(CustomNodeViewAttribute)) })
                .SingleOrDefault(item => item.Attribute != null && item.Attribute.Type == type);

            return nodeTypeAttribute?.Type;
        }

        public static bool TryGetCustomGraphViewNodeType(Type baseNodeType,out Type customNodeViewType)
        {
            var types = baseNodeType.Assembly.GetTypes();
            var typesWithAttribute = types
                .Where(t => t.IsSubclassOf(typeof(NodeView)) && IsDefined(t, typeof(CustomNodeViewAttribute)));

            var nodeTypeAttribute = typesWithAttribute
                .Select(t => new { Type = t, Attribute = (CustomNodeViewAttribute)GetCustomAttribute(t, typeof(CustomNodeViewAttribute)) })
                .SingleOrDefault(item => item.Attribute != null && item.Attribute.Type == baseNodeType);

            if(nodeTypeAttribute == null)
            {
                customNodeViewType = default;
                return false;
            }
            else
            {
                customNodeViewType = nodeTypeAttribute.Type;
                return true;
            }
        }
    }
}