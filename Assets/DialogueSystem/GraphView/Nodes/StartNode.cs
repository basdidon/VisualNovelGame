using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Linq;
using System;

namespace BasDidon.Dialogue.VisualGraphView
{
    public class StartNode : BaseNode,IExecutableNode
    {
        [Port(PortDirection.Output)]
        public ExecutionFlow output;

        public void OnEnter()
        {
            var connectedNodes = DialogueTree.GetConnectedNodes<IExecutableNode>(GetPortData("output"));
            Debug.Log(connectedNodes.Count());
            DialogueManager.Instance.CurrentNode = connectedNodes.FirstOrDefault();
        }

        public void OnExit(){}
    }
}