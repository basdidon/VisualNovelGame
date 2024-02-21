using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace BasDidon.Dialogue.VisualGraphView
{
    using BasDidon;
    using UnityEditor.UIElements;

    public static class NodeFactory
    {
        public static BaseNode CreateNode(Type type, Vector2 position, DialogueTree dialogueTree)
        {
            BaseNode nodeData = (BaseNode) ScriptableObject.CreateInstance(type);
            if(nodeData != null)
            {
                nodeData.Initialize(position, dialogueTree);

                return nodeData;
            }

            throw new Exception();
        }
        public static BaseNode CreateNode<T>(Vector2 position, DialogueTree dialogueTree) where T : BaseNode
        {
            var nodeData = ScriptableObject.CreateInstance<T>();
            nodeData.Initialize(position, dialogueTree);

            return nodeData;
        }

        public static NodeView GetNodeView<T>(T nodeData, DialogueGraphView graphView) where T : BaseNode
        {
            var customGraphViewNodeType = CustomGraphViewNodeAttribute.GetGraphViewNodeType(nodeData.GetType());

            NodeView instance;

            if (customGraphViewNodeType != null)
            {
                var nodeType = customGraphViewNodeType;
                instance = Activator.CreateInstance(nodeType) as NodeView;
                Debug.Log($"GetNodeView {nodeType.Name}");
            }
            else
            {
                instance = Activator.CreateInstance<NodeView>();
            }

            instance.Initialize(nodeData,graphView);
            instance.OnDrawNodeView(nodeData);
            return instance;
        }
    }

    public static class NodeElementFactory
    {
        public static void DrawPortWithField(SerializedProperty serializedProperty,Type type, PortData portData, NodeView nodeView, string propertyName)
        {
            if (portData.Direction == Direction.Input)
            {
                nodeView.inputContainer.Add(CreatePortWithField(serializedProperty,type, portData, nodeView,propertyName));
            }
            else // assume portData.Direction == Direction.Output
            {
                nodeView.outputContainer.Add(CreatePortWithField(serializedProperty,type, portData, nodeView,propertyName));
            }
        }

        public static Port CreatePortWithField(SerializedProperty serializedProperty, Type type, PortData portData, NodeView nodeView,string propertyName)
        {
            Port.Capacity capacity = portData.Direction == Direction.Input ? Port.Capacity.Single : Port.Capacity.Multi;

            var port = nodeView.InstantiatePort(Orientation.Horizontal, portData.Direction, capacity, type);
            port.viewDataKey = portData.PortGuid;
            port.portName = propertyName;

            var propertyField = new PropertyField(serializedProperty,string.Empty);

            if (portData.Direction == Direction.Input)
            {
                port.Insert(1, propertyField);

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

        public static void DrawPort(Type type,PortData portData,string bindingPath, NodeView nodeView)
        {
            if(portData.Direction == Direction.Input)
            {
                nodeView.inputContainer.Add(CreatePort(type,portData, bindingPath, nodeView));
            }
            else // assume portData.Direction == Direction.Output
            {
                nodeView.outputContainer.Add(CreatePort(type,portData, bindingPath, nodeView));
            }
        }

        public static VisualElement CreatePort(Type type,PortData portData,string bindingPath, NodeView nodeView)
        {
            var  propertyName = StringHelper.ToCapitalCase(StringHelper.GetBackingFieldName(bindingPath));

            if (type == typeof(ExecutionFlow))
            {
                return GetExecutionFlowPort(portData, nodeView, propertyName);
            }
            else if (type == typeof(bool))
            {
                return GetPortField(type, portData, nodeView, new Toggle() { bindingPath = bindingPath }, propertyName);
            }
            else if(type == typeof(string))
            {
                return GetPortField(type, portData, nodeView, new TextField() { bindingPath = bindingPath }, propertyName); 
            }
            
            throw new InvalidOperationException($"{type.Name} is not supported");
        }

        static Port GetPortField(Type type,PortData portData, NodeView nodeView, VisualElement fieldElement, string propertyName = null)
        {
            Port.Capacity capacity = portData.Direction == Direction.Input ? Port.Capacity.Single : Port.Capacity.Multi;

            var port = nodeView.InstantiatePort(Orientation.Horizontal, portData.Direction, capacity, type);
            port.viewDataKey = portData.PortGuid;
            port.portName = propertyName;

            var propertyField = new PropertyField();

            if (portData.Direction == Direction.Input)
            {
                port.Insert(1, fieldElement);

                nodeView.GraphView.OnPortConnect += (onConnectPort) =>
                {
                    if (port == onConnectPort)
                    {
                        fieldElement.style.display = DisplayStyle.None;
                    }
                };

                nodeView.GraphView.OnPortDisconnect += (onDisconnectPort) =>
                {
                    if (port == onDisconnectPort)
                    {
                        fieldElement.style.display = DisplayStyle.Flex;
                    }
                };
            }
            else
            {
                port.Add(fieldElement);
            }

            return port;
        }
        
        static VisualElement GetExecutionFlowPort(PortData portData, NodeView nodeView, string propertyName)
        {
            var port = nodeView.InstantiatePort(
                Orientation.Horizontal,
                portData.Direction,
                portData.Direction == Direction.Input ? Port.Capacity.Single : Port.Capacity.Multi,
                typeof(ExecutionFlow)
            );

            port.viewDataKey = portData.PortGuid;
            port.portName = propertyName;
            port.portColor = Color.yellow;

            return port;
        }
        /*
        static Port GetBoolPort(PortData portData, string bindingPath, NodeView nodeView, string propertyName = null)
        {
            Port.Capacity capacity = portData.Direction == Direction.Input ? Port.Capacity.Single : Port.Capacity.Multi;

            var port = nodeView.InstantiatePort(Orientation.Horizontal, portData.Direction, capacity, typeof(bool));
            port.viewDataKey = portData.PortGuid;
            port.portName = propertyName;

            var valueToggle = new Toggle() { bindingPath = bindingPath};

            if(portData.Direction == Direction.Input)
            {
                port.Insert(1, valueToggle);

                nodeView.GraphView.OnPortConnect += (onConnectPort) =>
                {
                    if (port == onConnectPort)
                    {
                        valueToggle.style.display = DisplayStyle.None;
                    }
                };

                nodeView.GraphView.OnPortDisconnect += (onDisconnectPort) =>
                {
                    if (port == onDisconnectPort)
                    {
                        valueToggle.style.display = DisplayStyle.Flex;
                    }
                };
            }
            else
            {
                port.Add(valueToggle);
            }

            return port;
        }


        static VisualElement GetStringPort(PortData portData, string bindingPath, NodeView nodeView, string propertyName = null)
        {
            var portElement = new VisualElement();
            portElement.style.flexDirection = portData.Direction == Direction.Input ? FlexDirection.Row : FlexDirection.RowReverse;

            var port = nodeView.InstantiatePort(Orientation.Horizontal, portData.Direction, Port.Capacity.Multi, typeof(string));
            port.viewDataKey = portData.PortGuid;
            port.portName = propertyName;
            portElement.Add(port);

            var textField = new TextField() { bindingPath = bindingPath };
            portElement.Add(textField);

            if (portData.Direction == Direction.Input)
            {
                nodeView.GraphView.OnPortConnect += (onConnectPort) =>
                {
                    if (port == onConnectPort)
                    {
                        textField.style.display = DisplayStyle.None;
                    }
                };

                nodeView.GraphView.OnPortDisconnect += (onDisconnectPort) =>
                {
                    if (port == onDisconnectPort)
                    {
                        textField.style.display = DisplayStyle.Flex;
                    }
                };
            }

            // style
            textField.style.flexGrow = 1;


            return portElement;
        }*/
    }
}