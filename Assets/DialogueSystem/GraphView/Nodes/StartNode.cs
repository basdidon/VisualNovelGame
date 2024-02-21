using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Linq;
using System;

namespace BasDidon.Dialogue.VisualGraphView
{
    public class StartNode : BaseNode,IExecutableNode
    {
        [field: Port(PortDirection.Output)]
        public ExecutionFlow Output { get; set; }

        public void OnEnter()
        {            
            DialogueManager.Instance.CurrentNode = DialogueTree.GetConnectedNodes<IExecutableNode>(GetPortData("Output")).First();
        }

        public void OnExit(){}
    }
}