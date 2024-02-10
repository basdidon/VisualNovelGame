using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;

namespace BasDidon.Dialogue.VisualGraphView
{
    [CustomGraphViewNode(typeof(StartNode))]
    public class CustomStartGraphViewNode : GraphViewNode
    {
        public override void OnDrawNodeView(BaseNode nodeData)
        {
            if (nodeData is StartNode startNode)
            {
                titleButtonContainer.style.display = DisplayStyle.None;
                // output port
                Debug.Log(startNode.OutputFlowPortData == null);
                Port outputPort = GetOutputFlowPort(startNode.OutputFlowPortData.PortGuid);
                outputContainer.Add(outputPort);

                RefreshExpandedState();
            }
        }
    }
}