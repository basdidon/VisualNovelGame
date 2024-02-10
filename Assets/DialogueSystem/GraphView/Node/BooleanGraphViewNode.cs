using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace BasDidon.Dialogue.VisualGraphView
{
    [CustomGraphViewNode(typeof(BooleanNode))]
    public class CustomBooleanGraphViewNode : GraphViewNode
    {
        public override void OnDrawNodeView(BaseNode nodeData)
        {
            if (nodeData is BooleanNode booleanNode)
            {
                var outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
                outputPort.viewDataKey = booleanNode.OutputFlowPortData.PortGuid;
                outputPort.portName = "bool";
                outputContainer.Add(outputPort);
                outputPort.userData = booleanNode.Value;

                var valueToggle = new Toggle()
                {
                    label = "boolean",
                    value = booleanNode.Value,
                    bindingPath = "<Value>k__BackingField"
                };

                extensionContainer.Add(valueToggle);
                RefreshExpandedState();
            }
        }
    }
}
