using Graphview.NodeView;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Graphview.NodeData
{
    public class StartNode : GVNodeData
    {
        [field: SerializeField] GVNodeData Child { get; set; }

        public override string[] InputPortGuids => new string[]{};
        public override string[] OutputPortGuids => new string[] { outputFlowPortData.PortGuid };

        // Port
        [SerializeField] PortData outputFlowPortData;

        public override void Initialize(Vector2 position, DialogueTree dialogueTree)
        {
            base.Initialize(position, dialogueTree);
            outputFlowPortData = new(dialogueTree, Direction.Output);
        }

        public override IEnumerable<GVNodeData> GetChildren()
        {
            return new GVNodeData[] { Child };
        }

        public override void Execute(){}
    }

    [CustomGraphViewNode(typeof(StartNode))]
    public class CustomStartGraphViewNode : GraphViewNode
    {
        public override void OnDrawNodeView(GVNodeData nodeData)
        {
            titleButtonContainer.style.display = DisplayStyle.None;
            // output port
            Port outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(ExecutionFlow));
            outputPort.viewDataKey = nodeData.OutputPortGuids[0];
            outputPort.portName = "Output";
            outputPort.portColor = Color.yellow;
            outputContainer.Add(outputPort);

            RefreshExpandedState();
        }
    }
}