using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BasDidon.Dialogue.VisualGraphView
{
    [System.Serializable]
    public class ExecutionFlow{}

    public class ExecutionFlowPortFactory : IPortFactory
    {
        public void BindPort(VisualElement e, string propertyName, string portGuid, PortAttribute portAttr,SerializedProperty serializedProperty = null)
        {
            Port port = e.Q<Port>(propertyName);
            if(port != null)
            {
                port.viewDataKey = portGuid;
            }
            else
            {
                Debug.LogError($"Port {propertyName} is not found.");
            }
        }

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

        public Port CreateUnbindPort(Direction direction, NodeView nodeView, string propertyName)
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

        public Port CreateUnbindPortWithField(Direction direction, NodeView nodeView, string propertyName)
        {
            return CreateUnbindPort(direction, nodeView, propertyName);
        }
    }
}
