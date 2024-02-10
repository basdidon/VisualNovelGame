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

        public PortData OutputFlowPortData { get; private set; }
        /*
        {
            get
            {
                var executable = (ExecutableNodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(ExecutableNodeAttribute), true);
                return DialogueTree.Nodes.SingleOrDefault(e=> e == executable.OutputPortGuid);
            }
        }*/

        public override void OnInstantiatePortData()
        {
            OutputFlowPortData = InstantiatePortData(Direction.Output);
        }

        public void OnEnter()
        {
            DialogueManager.Instance.CurrentNode = OutputFlowPortData.GetConnectedNodeOfType<IExecutableNode>().FirstOrDefault();
        }

        public void OnExit(){}
    }
}