using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BasDidon.Dialogue.VisualGraphView
{
    public class PrimativeTypePortFactory : IPortFactory
    {
        static readonly Type[] competableTypes = new[] { typeof(bool), typeof(string), typeof(int) };

        public Type Type { get; }

        PrimativeTypePortFactory(Type type)
        {
            Type = type;
        }

        public static bool TryCreateFactory(Type type, out PrimativeTypePortFactory factory)
        {
            factory = default;

            if (competableTypes.Contains(type))
            {
                factory = new(type);
                return true;
            }

            return false;
        }

        public Port CreatePort(string portGuid, Direction direction, NodeView nodeView, string portName)
        {
            Port.Capacity capacity = direction == Direction.Input ? Port.Capacity.Single : Port.Capacity.Multi;

            var port = nodeView.InstantiatePort(Orientation.Horizontal, direction, capacity, Type);
            port.viewDataKey = portGuid;
            port.portName = portName;

            return port;
        }

        public Port CreatePortWithField(SerializedProperty serializedProperty, string portGuid, Direction direction, NodeView nodeView, string propertyName)
        {
            var port = CreatePort(portGuid, direction, nodeView, propertyName);
            AddFieldToPort(serializedProperty, port, nodeView);

            return port;
        }


        static void AddFieldToPort(SerializedProperty serializedProperty, Port port, NodeView nodeView)
        {
            var propertyField = new PropertyField(serializedProperty, string.Empty);
            propertyField.BindProperty(serializedProperty);

            if (port.direction == Direction.Input)
            {
                //port.Insert(1, propertyField);

                port.Add(propertyField);
                nodeView.GraphView.OnPortConnect += (onConnectPort) =>
                {
                    if (port == onConnectPort)
                    {
                        propertyField.style.display = DisplayStyle.None;
                    }
                };

                nodeView.GraphView.OnPortDisconnect += (onDisconnectPort) =>
                {
                    if (port == onDisconnectPort)
                    {
                        propertyField.style.display = DisplayStyle.Flex;
                    }
                };
            }
            else
            {
                port.Add(propertyField);
            }
        }

        public Port CreateUnbindPort(Direction direction, NodeView nodeView, string portName)
        {
            Port.Capacity capacity = direction == Direction.Input ? Port.Capacity.Single : Port.Capacity.Multi;

            var port = nodeView.InstantiatePort(Orientation.Horizontal, direction, capacity, Type);
            port.portName = StringHelper.ToCapitalCase(portName);
            port.name = portName;

            return port;
        }

        public Port CreateUnbindPortWithField(Direction direction, NodeView nodeView, string propertyName)
        {
            var port = CreateUnbindPort(direction, nodeView, propertyName);
            var propertyField = new PropertyField()
            {
                name = $"{propertyName}_BackingField",
                label = string.Empty
            };
            

            if (port.direction == Direction.Input)
            {
                //port.Insert(1, propertyField);

                port.Add(propertyField);
                nodeView.GraphView.OnPortConnect += (onConnectPort) =>
                {
                    if (port == onConnectPort)
                    {
                        propertyField.style.display = DisplayStyle.None;
                    }
                };

                nodeView.GraphView.OnPortDisconnect += (onDisconnectPort) =>
                {
                    if (port == onDisconnectPort)
                    {
                        propertyField.style.display = DisplayStyle.Flex;
                    }
                };
            }
            else
            {
                port.Add(propertyField);
            }

            return port;
        }

        public void BindPort(VisualElement e,string fieldName,string portGuid,PortAttribute portAttr, SerializedProperty serializedProperty = null)
        {
            var port = e.Q<Port>(fieldName);
            if (port != null)
            {
                port.viewDataKey = portGuid ?? string.Empty;//?? throw new KeyNotFoundException();
            }

            Debug.Log($"{portAttr.HasBackingFieldName}:{serializedProperty != null}");

            if (portAttr.HasBackingFieldName && serializedProperty != null)
            {
                Debug.Log("pass");
                var backingElementName = $"{portAttr.BackingFieldName}_BackingField";
                var backingFieldElement = e.Q<PropertyField>(backingElementName);
                if (backingFieldElement != null)
                {
                    Debug.Log("pass x2");
                    backingFieldElement.BindProperty(serializedProperty);
                }
                else
                {

                }

            }
        }
    }
}