using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEditor;

namespace H8.GraphView
{
    using H8.GraphView.UiElements;

    [AttributeUsage(AttributeTargets.Property)]
    public abstract class PortAttribute : Attribute
    {
        public string BackingFieldName { get; }
        public bool HasBackingFieldName => !string.IsNullOrEmpty(BackingFieldName);
        public abstract Direction Direction { get; }

        public PortAttribute(string backingFieldName = null)
        {
            BackingFieldName = backingFieldName;
        }

        public static bool TryGetDirectionFromPropertyInfo(PropertyInfo propertyInfo, out Direction direction)
        {
            if (propertyInfo.IsDefined(typeof(PortAttribute)))
            {
                PortAttribute portAttr = propertyInfo.GetCustomAttribute<PortAttribute>();
                direction = portAttr.Direction;
                return true;
            }

            direction = default;
            return false;
        }

        public static IEnumerable<PortData> CreatePortsData(BaseListElement listElement) => CreatePortsData(listElement.GetType());
        public static IEnumerable<PortData> CreatePortsData(BaseNode baseNode) => CreatePortsData(baseNode.GetType());
        static IEnumerable<PortData> CreatePortsData(Type type)
        {
            Debug.Log(type.Name);
            var properties = type.GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (TryGetDirectionFromPropertyInfo(property, out Direction direction))
                {
                    // use Direction from PortAtrribute to create PortData
                    PortData newPortData = new(direction, property.Name);

                    Debug.Log($"added new {newPortData.Direction} Port : {property.Name} {newPortData.PortGuid}");
                    yield return newPortData;
                }
            }
        }
    }
}
