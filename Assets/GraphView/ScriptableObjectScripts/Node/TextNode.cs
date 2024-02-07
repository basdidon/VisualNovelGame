using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

namespace Graphview.NodeData
{
    using NodeView;
    public class TextNode : NodeData
    {
        [field:SerializeField] public PortData OutputStringPortData { get; private set; }

        public override void Initialize(Vector2 position, DialogueTree dialogueTree)
        {
            base.Initialize(position, dialogueTree);
            
            values = new();
            values.Add(new OutputValue() { Guid = OutputStringPortData.PortGuid, Value = "chipi chipi" , Text = "chipi chipi"});
            Debug.Log("chipi chipi");
        }

        public override void OnInstantiatePortData()
        {
            OutputStringPortData = InstantiatePortData(Direction.Output);
        }
    }

    [CustomGraphViewNode(typeof(TextNode))]
    public class CustomTextGraphViewNode : GraphViewNode
    {
        public override void OnDrawNodeView(NodeData nodeData)
        {
            if(nodeData is TextNode textNode)
            {
                var outputStringPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(string));
                outputStringPort.viewDataKey = textNode.OutputStringPortData.PortGuid;
                outputContainer.Add(outputStringPort);
            }
        }
    }
}