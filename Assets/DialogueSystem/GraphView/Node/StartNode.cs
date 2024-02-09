using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Linq;

namespace BasDidon.Dialogue.VisualGraphView
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
}