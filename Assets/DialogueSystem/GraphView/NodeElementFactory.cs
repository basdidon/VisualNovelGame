using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BasDidon.Dialogue.VisualGraphView
{
    public static class NodeElementFactory
    {
        public static void DrawPort(Type type, PortData portData, NodeView nodeView, string portName)
        {
            VisualElement portContainer = portData.Direction == Direction.Input ? nodeView.inputContainer : nodeView.outputContainer;
            portContainer.Add(CreatePort(type,portData,nodeView,portName));
        }

        public static Port CreatePort(Type type, PortData portData, NodeView nodeView, string portName)
            => CreatePort(type, portData.PortGuid, portData.Direction, nodeView, portName);

        public static Port CreatePort(Type type, string portGuid, Direction direction, NodeView nodeView, string portName)
        {
            Type[] usePropotyFieldTypes = new[] { typeof(bool), typeof(string), typeof(int) };

            if (type == typeof(ExecutionFlow))
            {
                return GetExecutionFlowPort(portGuid, direction, nodeView, portName);
            }
            else if (usePropotyFieldTypes.Contains(type))
            {
                Port.Capacity capacity = direction == Direction.Input ? Port.Capacity.Single : Port.Capacity.Multi;

                var port = nodeView.InstantiatePort(Orientation.Horizontal, direction, capacity, type);
                port.viewDataKey = portGuid;
                port.portName = portName;

                return port;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
        
        ///
        public static void DrawPortWithField(SerializedProperty serializedProperty, Type type, PortData portData, NodeView nodeView, string propertyName, PortFieldStyle portFieldStyle = PortFieldStyle.Show)
        {
            VisualElement portContainer = portData.Direction == Direction.Input ? nodeView.inputContainer : nodeView.outputContainer;
            portContainer.Add(CreatePortWithField(serializedProperty, type, portData, nodeView, propertyName, portFieldStyle));
        }

        // CreatePortWithField
        public static Port CreatePortWithField(SerializedProperty serializedProperty, Type type, PortData portData, NodeView nodeView, string propertyName, PortFieldStyle portFieldStyle = PortFieldStyle.Show)
        {
            return CreatePortWithField(serializedProperty, type, portData.PortGuid, portData.Direction, nodeView, propertyName, portFieldStyle);
        }

        public static Port CreatePortWithField(SerializedProperty serializedProperty, Type type, string portGuid, Direction direction, NodeView nodeView, string propertyName, PortFieldStyle portFieldStyle = PortFieldStyle.Show)
        {
            Type[] usePropotyFieldTypes = new[] { typeof(bool), typeof(string), typeof(int) };

            if (type == typeof(ExecutionFlow))
            {
                return GetExecutionFlowPort(portGuid, direction, nodeView, propertyName);
            }
            else if (usePropotyFieldTypes.Contains(type))
            {
                Port.Capacity capacity = direction == Direction.Input ? Port.Capacity.Single : Port.Capacity.Multi;

                var port = nodeView.InstantiatePort(Orientation.Horizontal, direction, capacity, type);
                port.viewDataKey = portGuid;
                port.portName = propertyName;

                if (portFieldStyle == PortFieldStyle.Show)
                {
                    // add field to port
                    AddFieldToPort(serializedProperty, port, nodeView);
                }

                return port;
            }
            else
            {
                throw new InvalidOperationException();
            }
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

        // GetExecutionFlowPort
        static Port GetExecutionFlowPort(string portGuid, Direction portDirection, NodeView nodeView, string propertyName)
        {
            var port = nodeView.InstantiatePort(
                Orientation.Horizontal,
                portDirection,
                portDirection == Direction.Output ? Port.Capacity.Single : Port.Capacity.Multi,
                typeof(ExecutionFlow)
            );

            port.viewDataKey = portGuid;
            port.portName = propertyName;
            port.portColor = Color.yellow;

            return port;
        }
    }
}