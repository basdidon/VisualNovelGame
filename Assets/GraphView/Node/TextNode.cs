using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

namespace Graphview.NodeData
{
    using NodeView;
    public class TextNode : GVNodeData
    {
        [field:SerializeField] public PortData OutputStringPortData { get; private set; }
        /*
        public override void Execute()
        {
            throw new System.NotImplementedException();
        }*/

        public override void Initialize(Vector2 position, DialogueTree dialogueTree)
        {
            base.Initialize(position, dialogueTree);

            

            values = new();
            values.Add(OutputStringPortData.PortGuid,"chipi chipi");
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
        public override void OnDrawNodeView(GVNodeData nodeData)
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