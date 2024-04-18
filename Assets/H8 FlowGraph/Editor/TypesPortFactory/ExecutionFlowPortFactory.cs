using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace H8.FlowGraph.UiElements
{
    public class ExecutionFlowPortFactory : IPortFactory
    {
        public Port CreatePort(string portGuid, Direction direction, NodeView nodeView, string portName)
        {
            Debug.Log($"Create Port ({GetType().Name}) : [{portName}] {portGuid}");
            var port = nodeView.InstantiatePort(
                Orientation.Horizontal,
                direction,
                direction == Direction.Output ? Port.Capacity.Single : Port.Capacity.Multi,
                typeof(ExecutionFlow)
            );

            port.viewDataKey = portGuid;
            port.portName = portName;
            port.portColor = Color.yellow;

            return port;
        }

        public Port CreatePortWithField(SerializedProperty serializedProperty, string portGuid, Direction direction, NodeView nodeView, string propertyName)
        {
            return CreatePort(portGuid,direction,nodeView,propertyName);
        }

        public Port CreateUnbindPort(Direction direction, NodeView nodeView, string propertyName,bool isHasBackingField)
        {
            var port = nodeView.InstantiatePort(
                Orientation.Horizontal,
                direction,
                direction == Direction.Output ? Port.Capacity.Single : Port.Capacity.Multi,
                typeof(ExecutionFlow)
            );

            port.portName = propertyName;
            port.name = propertyName;
            port.portColor = Color.yellow;

            return port;
        }

        public void BindPort(VisualElement e, string propertyName, string portGuid, PortAttribute portAttr, SerializedProperty serializedProperty = null)
        {
            Port port = e.Q<Port>(propertyName);

            if (port == null)
                throw new System.Exception($"Port {propertyName} is not found.");

            port.viewDataKey = portGuid;
        }
    }
}
