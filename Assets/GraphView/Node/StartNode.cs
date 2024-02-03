using Graphview.NodeView;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

namespace Graphview.NodeData
{
    public class StartNode : GVNodeData
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
        /*
        public override void Execute(int idx)
        {
            DialogueManager.Instance.CurrentNode = OutputFlowPortData.ConnectedNode.FirstOrDefault();
        }*/

    }

    [CustomGraphViewNode(typeof(StartNode))]
    public class CustomStartGraphViewNode : GraphViewNode
    {
        public override void OnDrawNodeView(GVNodeData nodeData)
        {
            if (nodeData is StartNode startNode)
            {
                titleButtonContainer.style.display = DisplayStyle.None;
                // output port
                Port outputPort = GetOutputFlowPort(startNode.OutputFlowPortData.PortGuid);
                outputContainer.Add(outputPort);

                RefreshExpandedState();
            }
        }
    }
}