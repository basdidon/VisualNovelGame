using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Linq;
using System;

namespace BasDidon.Dialogue.VisualGraphView
{
    public class StartNode : BaseNode,IExecutableNode
    {
        [Output]
        public ExecutionFlow Output { get; }

        public void OnEnter()
        {
            var connectedNodes = DialogueTree.GetConnectedNodes<IExecutableNode>(GetPortData(nameof(Output)));
            Debug.Log(connectedNodes.Count());
            DialogueManager.Instance.CurrentNode = connectedNodes.FirstOrDefault();
        }

        public void OnExit(){}
    }
}