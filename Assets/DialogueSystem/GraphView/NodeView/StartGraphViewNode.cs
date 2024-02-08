using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace BasDidon.Dialogue.VisualGraphView
{
    [CustomGraphViewNode(typeof(StartNode))]
    public class CustomStartGraphViewNode : GraphViewNode
    {
        public override void OnDrawNodeView(NodeData nodeData)
        {
            if (nodeData is StartNode startNode)
            {
                titleButtonContainer.style.display = DisplayStyle.None;
                // output port
                Port outputPort = GetOutputFlowPort(startNode.OutputFlowPortData.PortGuid);
                outputContainer.Add(outputPort);

                RefreshExpandedState();
            }
        }
    }
}