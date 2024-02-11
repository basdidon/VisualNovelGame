using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Linq;
using System;

namespace BasDidon.Dialogue.VisualGraphView
{
    //[ExecutableNode(false)]
    public class StartNode : BaseNode,IExecutableNode
    {
        [Output] public ExecutionFlow Output { get; set; }

        // Port
        [field:SerializeField] public PortData OutputFlowPortData { get; private set; }

        public override void OnInstantiatePortData()
        {
            OutputFlowPortData = InstantiatePortData(Direction.Output);
        }

        public void OnEnter()
        {
            DialogueManager.Instance.CurrentNode = DialogueTree.GetConnectedNodes<IExecutableNode>(OutputFlowPortData).First();
        }

        public void OnExit(){}
    }
}