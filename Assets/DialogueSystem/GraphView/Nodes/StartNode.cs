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
            GraphTreeContorller.Instance.ToNextExecutableNode(GetPortData(nameof(Output)), DialogueTree);
        }

        public void OnExit(){}

        public void Action(IBaseAction action){}
    }
}