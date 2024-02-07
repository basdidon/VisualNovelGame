using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEditor;

namespace Graphview.NodeData
{
    using NodeView;

    public class BooleanNode : NodeData
    {
        [SerializeField] bool value;
        [field:SerializeField] public bool Value { get; set; }

        [field: SerializeField] public PortData OutputFlowPortData { get; private set; }

        public override void Initialize(Vector2 position, DialogueTree dialogueTree)
        {
            base.Initialize(position, dialogueTree);

            SaveChanges();
        }

        public override void OnInstantiatePortData()
        {
            OutputFlowPortData = InstantiatePortData(Direction.Output);
        }
        /*
        public override void Execute() { }
        */
    }

    [CustomGraphViewNode(typeof(BooleanNode))]
    public class CustomBooleanGraphViewNode : GraphViewNode
    {
        public override void OnDrawNodeView(NodeData nodeData)
        {
            if(nodeData is BooleanNode booleanNode)
            {
                var outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
                outputPort.viewDataKey = booleanNode.OutputFlowPortData.PortGuid;
                outputPort.portName = "bool";
                outputContainer.Add(outputPort);
                outputPort.userData = booleanNode.Value;

                var valueToggle = new Toggle() { 
                    label = "boolean", 
                    value = booleanNode.Value,
                    bindingPath = "<Value>k__BackingField"
                };
                
                extensionContainer.Add(valueToggle);
                RefreshExpandedState();
            }
        }
    }

    public class BooleanPortOutput
    {
        public bool GetValue;
    }

    public class PortOutput<T>
    {
        public T Value { get; set; }
    }
}