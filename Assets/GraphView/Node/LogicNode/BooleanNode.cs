using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;

namespace Graphview.NodeData
{
    using NodeView;
    public class BooleanNode : GVNodeData
    {
        [field:SerializeField] public bool Value { get; set; }

        [field: SerializeField] GVNodeData Child { get; set; }

        public override string[] InputPortGuids => new string[] { };

        [SerializeField] string outputPortGuid;
        public override string[] OutputPortGuids => new string[] { outputPortGuid };

        public override void AddChild(GVNodeData child)
        {
            Child = child;
        }

        public override void RemoveChild(GVNodeData child)
        {
            if (Child == child)
                Child = null;
        }

        public override IEnumerable<GVNodeData> GetChildren()
        {
            return new GVNodeData[] { Child };
        }

        public override void Execute() { }
    }

    [CustomGraphViewNode(typeof(BooleanNode))]
    public class CustomBooleanGraphViewNode : GraphViewNode
    {
        public override void OnDrawNodeView(GVNodeData nodeData)
        {
            if(NodeData is BooleanNode booleanNode)
            {
                var outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
                outputPort.viewDataKey = NodeData.OutputPortGuids[0];
                outputPort.portName = "bool";
                outputContainer.Add(outputPort);

                var valueToggle = new Toggle() { label = "boolean", value = booleanNode.Value };
                valueToggle.RegisterValueChangedCallback(e => booleanNode.Value = e.newValue);
                extensionContainer.Add(valueToggle);
                RefreshExpandedState();
            }
        }
    }
}