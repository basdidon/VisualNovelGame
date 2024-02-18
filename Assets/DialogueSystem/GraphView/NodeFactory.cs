using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace BasDidon.Dialogue.VisualGraphView
{
    using BasDidon;

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
            Debug.Log("Factory OnDrawNodeView");
            instance.OnDrawNodeView(nodeData);
            return instance;
        }
    }

    public static class NodeElementFactory
    {
        public static void DrawPort(PortData portData,string bindingPath, NodeView nodeView)
        {
            if(portData.Direction == Direction.Input)
            {
                nodeView.inputContainer.Add(GetPort(portData, bindingPath, nodeView));
            }
            else // assume portData.Direction == Direction.Output
            {
                nodeView.outputContainer.Add(GetPort(portData, bindingPath, nodeView));
            }
        }

        public static VisualElement GetPort(PortData portData,string bindingPath, NodeView nodeView)
        {
            var  propertyName = StringHelper.ToCapitalCase(StringHelper.GetBackingFieldName(bindingPath));

            if (portData.Type == typeof(ExecutionFlow))
            {
                return GetExecutionFlowPort(portData, nodeView, propertyName);
            }
            else if (portData.Type == typeof(bool))
            {
                return GetBoolPort(portData, bindingPath, nodeView,  propertyName);
            }
            else if(portData.Type == typeof(string))
            {
                return GetStringPort(portData, bindingPath, nodeView, propertyName); 
            }
            
            throw new InvalidOperationException($"{portData.Type.Name} is not supported");
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

        static VisualElement GetBoolPort(PortData portData, string bindingPath, NodeView nodeView, string propertyName = null)
        {
            var portElement = new VisualElement();
            portElement.style.flexDirection = portData.Direction == Direction.Input ? FlexDirection.Row : FlexDirection.RowReverse;

            var port = nodeView.InstantiatePort(Orientation.Horizontal, portData.Direction, Port.Capacity.Multi, typeof(bool));
            port.viewDataKey = portData.PortGuid;
            port.portName =propertyName;
            portElement.Add(port);

            var valueToggle = new Toggle() { bindingPath = bindingPath};
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
        }
    }
}