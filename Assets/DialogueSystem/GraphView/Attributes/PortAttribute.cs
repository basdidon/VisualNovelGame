using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEditor;

namespace BasDidon.Dialogue.VisualGraphView
{
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
            bool isInput = propertyInfo.IsDefined(typeof(InputAttribute));
            bool isOutput = propertyInfo.IsDefined(typeof(OutputAttribute));

            if (isInput ^ isOutput) // XOR
            {
                direction = default;
                return false;
            }

            if (isInput)
            {
                direction = Direction.Input;
            }
            else
            {
                direction = Direction.Output;
            }

            return true;
        }

        public static IEnumerable<PortData> CreatePortsData(ListElement listElement) => CreatePortsData(listElement.GetType());
        public static IEnumerable<PortData> CreatePortsData(BaseNode baseNode) => CreatePortsData(baseNode.GetType());
        static IEnumerable<PortData> CreatePortsData(Type type)
        {
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
        public static Port CreateUnbindPort(PropertyInfo propertyInfo, NodeView nodeView)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException();

            var portAttr = propertyInfo.GetCustomAttribute<PortAttribute>();

            if (portAttr == null)
                throw new InvalidOperationException();

            Type type = propertyInfo.PropertyType;
            IPortFactory portFactory = GetPortFactory(type);

            if (portAttr.HasBackingFieldName)
            {
                return portFactory.CreateUnbindPortWithField(portAttr.Direction, nodeView, propertyInfo.Name);
            }
            else
            {
                return portFactory.CreateUnbindPort(portAttr.Direction, nodeView, propertyInfo.Name);
            }
        }

        public static Port CreatePort(PropertyInfo propertyInfo, NodeView nodeView, PortData portData, SerializedObject serializedObject = null)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException();

            var portAttr = propertyInfo.GetCustomAttribute<PortAttribute>();

            if (portAttr == null)
                throw new InvalidOperationException();

            Type type = propertyInfo.PropertyType;
            IPortFactory portFactory = GetPortFactory(type);

            if (serializedObject != null && portAttr.HasBackingFieldName)
            {
                var serializeProperty = serializedObject.FindProperty(portAttr.BackingFieldName);

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

        // find match Factory class for specific type
        static IPortFactory GetPortFactory(Type type)
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
                CustomTypeAttribute customTypeAttr = Attribute.GetCustomAttribute(type, typeof(CustomTypeAttribute)) as CustomTypeAttribute;
                return Activator.CreateInstance(customTypeAttr.PortFactoryType.GetType()) as IPortFactory;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}
