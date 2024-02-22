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
            Debug.Log(GetPortData("<Output>k__BackingField").Direction);
            var connectedNodes = DialogueTree.GetConnectedNodes<IExecutableNode>(GetPortData("<Output>k__BackingField"));
            Debug.Log(connectedNodes.Count());
            DialogueManager.Instance.CurrentNode = connectedNodes.FirstOrDefault();
        }

        public void OnExit(){}
    }
}