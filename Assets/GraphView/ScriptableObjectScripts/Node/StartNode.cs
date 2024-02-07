using Graphview.NodeView;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

namespace Graphview.NodeData
{
    public class StartNode : NodeData,IExecutableNode
    {
        // Port
        [field:SerializeField] public PortData OutputFlowPortData { get; private set; }

        public override void Initialize(Vector2 position, DialogueTree dialogueTree)
        {
            base.Initialize(position, dialogueTree);
        }

        public override void OnInstantiatePortData()
        {
            OutputFlowPortData = InstantiatePortData(Direction.Output);
        }

        public void Start()
        {
            DialogueManager.Instance.CurrentNode = OutputFlowPortData.GetConnectedNodeOfType<IExecutableNode>().FirstOrDefault();
            Debug.Log(OutputFlowPortData.GetConnectedNodeOfType<IExecutableNode>().FirstOrDefault());
        }

        public void Exit(){}
    }

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