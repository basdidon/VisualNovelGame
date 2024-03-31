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

        // View
        public Port CreateUnbindPort(PropertyInfo propertyInfo, NodeView nodeView)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException();

            Type type = propertyInfo.PropertyType;
            IPortFactory portFactory = PortFactoryUtils.GetPortFactory(type);

            return portFactory.CreateUnbindPort(Direction, nodeView, propertyInfo.Name, HasBackingFieldName);
        }

        public Port CreatePort(PropertyInfo propertyInfo, NodeView nodeView, PortData portData, SerializedObject serializedObject = null)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException();

            Type type = propertyInfo.PropertyType;
            IPortFactory portFactory = PortFactoryUtils.GetPortFactory(type);

            if (serializedObject != null && HasBackingFieldName)
            {
                var serializeProperty = serializedObject.FindProperty(BackingFieldName);

                if (serializeProperty == null)
                    throw new NullReferenceException();
                else
                    return portFactory.CreatePortWithField(serializeProperty, portData.PortGuid, portData.Direction, nodeView, propertyInfo.Name);
            }
            else
            {
                return portFactory.CreatePort(portData.PortGuid, portData.Direction, nodeView, propertyInfo.Name);
            }
        }
        /*
        // find match Factory class for specific type
        public static IPortFactory GetPortFactory(Type type)
        {
            if (type == typeof(ExecutionFlow))
            {
                return new ExecutionFlowPortFactory();
            }
            else if (PrimativeTypePortFactory.TryCreateFactory(type, out PrimativeTypePortFactory factory))
            {
                return factory;
            }
            else if (type.IsDefined(typeof(CustomTypeAttribute), true))
            {
                CustomTypeAttribute customTypeAttr = GetCustomAttribute(type, typeof(CustomTypeAttribute)) as CustomTypeAttribute;
                return Activator.CreateInstance(customTypeAttr.PortFactoryType.GetType()) as IPortFactory;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }*/
    }
}
