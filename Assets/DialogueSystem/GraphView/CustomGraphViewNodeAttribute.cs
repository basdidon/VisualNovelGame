using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

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

        public static Type GetGraphViewNodeType(Type type)  
        {
            var allTypes = Assembly.GetExecutingAssembly().GetTypes();
            var typesWithAttribute = allTypes
                .Where(t => t.IsSubclassOf(typeof(GraphViewNode)) && IsDefined(t, typeof(CustomGraphViewNodeAttribute)));

            Debug.Log(typesWithAttribute.Count());

            var nodeTypeAttribute = typesWithAttribute
                .Select(t => new { Type = t, Attribute = (CustomGraphViewNodeAttribute) GetCustomAttribute(t, typeof(CustomGraphViewNodeAttribute)) })
                .SingleOrDefault(item => item.Attribute != null && item.Attribute.Type == type);

            return nodeTypeAttribute?.Type;
        }
    }
}