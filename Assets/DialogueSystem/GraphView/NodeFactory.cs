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
        public static BaseNode CreateNode<T>(Vector2 position, DialogueTree dialogueTree) where T : BaseNode
        {
            var nodeData = ScriptableObject.CreateInstance<T>();
            nodeData.Initialize(position, dialogueTree);

            return nodeData;
        }

        public static GraphViewNode GetNodeView<T>(T nodeData, DialogueGraphView graphView) where T : BaseNode
        {
            var allTypes = Assembly.GetExecutingAssembly().GetTypes();
            var typesWithAttribute = allTypes
                .Where(t => t.IsSubclassOf(typeof(GraphViewNode)) && Attribute.IsDefined(t, typeof(CustomGraphViewNodeAttribute)));

            var nodeTypeAttribute = typesWithAttribute
                .Select(t => new { Type = t, Attribute = (CustomGraphViewNodeAttribute)Attribute.GetCustomAttribute(t, typeof(CustomGraphViewNodeAttribute)) })
                .SingleOrDefault(item => item.Attribute != null && item.Attribute.Type == nodeData.GetType());

            GraphViewNode instance;

            if (nodeTypeAttribute != null)
            {
                var nodeType = nodeTypeAttribute.Type;
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

    public static class NodePortFactory
    {
        public static VisualElement GetPort<T>(PortData portData, string propertyName, GraphViewNode nodeView) 
        {
            return GetPort(typeof(T), portData, propertyName, nodeView);
        }

        public static VisualElement GetPort(Type type, PortData portData, string propertyName, GraphViewNode nodeView)
        {
            if (type == typeof(ExecutionFlow))
            {
                return GetExecutionFlowPort(portData, propertyName, nodeView);
            }
            else if (type == typeof(bool))
            {
                return GetBoolPort(portData, propertyName, nodeView);
            }


            throw new InvalidOperationException();
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
            port.portName = propertyName;
            port.portColor = Color.yellow;

            return port;
        }

        static VisualElement GetBoolPort(PortData portData,string propertyName,GraphViewNode nodeView)
        {
            var portElement = new VisualElement();
            portElement.style.flexDirection = portData.Direction == Direction.Input ? FlexDirection.Row : FlexDirection.RowReverse;

            var port = nodeView.InstantiatePort(Orientation.Horizontal, portData.Direction, Port.Capacity.Multi,typeof(bool));
            port.viewDataKey = portData.PortGuid;
            port.portName = propertyName;
            portElement.Add(port);
            
            var valueToggle = new Toggle()
            {
                bindingPath = $"<{propertyName}>k__BackingField"
            };

            portElement.Add(valueToggle);

            return portElement;
        }
    }
}