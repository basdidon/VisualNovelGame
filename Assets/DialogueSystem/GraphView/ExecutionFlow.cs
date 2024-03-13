using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BasDidon.Dialogue.VisualGraphView
{
    [System.Serializable]
    public class ExecutionFlow{}

    public class ExecutionFlowPortFactory : IPortFactory
    {
        public Port CreatePort(string portGuid, Direction direction, NodeView nodeView, string portName)
        {
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

        public Port CreateUnbindPort(Direction direction, NodeView nodeView, string portName)
        {
            var port = nodeView.InstantiatePort(
                Orientation.Horizontal,
                direction,
                direction == Direction.Output ? Port.Capacity.Single : Port.Capacity.Multi,
                typeof(ExecutionFlow)
            );

            port.portName = portName;
            port.portColor = Color.yellow;

            return port;
        }

        public Port CreateUnbindPortWithField(Direction direction, NodeView nodeView, string propertyName)
        {
            return CreateUnbindPort(direction, nodeView, propertyName);
        }
    }
}
