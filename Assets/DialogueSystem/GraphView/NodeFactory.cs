using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace BasDidon.Dialogue.VisualGraphView
{
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

        public static GraphViewNode GetNodeView<T>(T nodeData, DialogueGraphView graphView) where T : BaseNode
        {
            var customGraphViewNodeType = CustomGraphViewNodeAttribute.GetGraphViewNodeType(nodeData.GetType());

            GraphViewNode instance;

            if (customGraphViewNodeType != null)
            {
                var nodeType = customGraphViewNodeType;
                instance = Activator.CreateInstance(nodeType) as GraphViewNode;
                Debug.Log($"GetNodeView {nodeType.Name}");
            }
            else
            {
                instance = Activator.CreateInstance<GraphViewNode>();
            }

            instance.Initialize(nodeData,graphView);
            instance.OnDrawNodeView(nodeData);
            return instance;
        }
    }

    public static class NodeElementFactory
    {
        public static VisualElement GetPort<T>(PortData portData, string propertyName, GraphViewNode nodeView, string bindingPath = null) 
        {
            return GetPort(typeof(T), portData, propertyName, nodeView, bindingPath);
        }

        public static VisualElement GetPort(Type type, PortData portData, string propertyName, GraphViewNode nodeView, string bindingPath = null)
        {
            if (type == typeof(ExecutionFlow))
            {
                return GetExecutionFlowPort(portData, propertyName, nodeView);
            }
            else if (type == typeof(bool))
            {
                return GetBoolPort(portData, propertyName, nodeView, bindingPath);
            }
            else if(type == typeof(string))
            {
                return GetStringPort(portData, propertyName, nodeView, bindingPath); 
            }
            

            throw new InvalidOperationException();
        }

        static string ToCapitalCase(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
            return char.ToUpper(text[0]) + text[1..];
        }

        static VisualElement GetExecutionFlowPort(PortData portData, string propertyName, GraphViewNode nodeView)
        {
            var port = nodeView.InstantiatePort(
                Orientation.Horizontal,
                portData.Direction,
                portData.Direction == Direction.Input ? Port.Capacity.Single : Port.Capacity.Multi,
                typeof(ExecutionFlow)
            );

            port.viewDataKey = portData.PortGuid;
            port.portName = ToCapitalCase(propertyName);
            port.portColor = Color.yellow;

            return port;
        }

        static VisualElement GetBoolPort(PortData portData, string propertyName, GraphViewNode nodeView, string bindingPath = null)
        {
            var portElement = new VisualElement();
            portElement.style.flexDirection = portData.Direction == Direction.Input ? FlexDirection.Row : FlexDirection.RowReverse;

            var port = nodeView.InstantiatePort(Orientation.Horizontal, portData.Direction, Port.Capacity.Multi, typeof(bool));
            port.viewDataKey = portData.PortGuid;
            port.portName = ToCapitalCase(propertyName);
            portElement.Add(port);

            var valueToggle = new Toggle() { bindingPath = bindingPath ?? propertyName };
            portElement.Add(valueToggle);

            if(portData.Direction == Direction.Input)
            {
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
            
            return portElement;
        }


        static VisualElement GetStringPort(PortData portData, string propertyName, GraphViewNode nodeView, string bindingPath = null)
        {
            var portElement = new VisualElement();
            portElement.style.flexDirection = portData.Direction == Direction.Input ? FlexDirection.Row : FlexDirection.RowReverse;

            var port = nodeView.InstantiatePort(Orientation.Horizontal, portData.Direction, Port.Capacity.Multi, typeof(string));
            port.viewDataKey = portData.PortGuid;
            port.portName = ToCapitalCase(propertyName);
            portElement.Add(port);

            var textField = new TextField() { bindingPath = bindingPath ?? propertyName };
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
        }
    }
}