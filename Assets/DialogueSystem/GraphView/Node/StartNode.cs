using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Linq;
using System;

namespace BasDidon.Dialogue.VisualGraphView
{
    //[ExecutableNode(false)]
    public class StartNode : BaseNode,IExecutableNode
    {
        [Output] public ExecutionFlow Output;

        public void OnEnter()
        {            
            DialogueManager.Instance.CurrentNode = DialogueTree.GetConnectedNodes<IExecutableNode>(GetPortData("Output")).First();
        }

        public void OnExit(){}
    }
}