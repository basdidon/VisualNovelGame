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

    public class BooleanNode : GVNodeData
    {
        [SerializeField] bool value;
        [field:SerializeField] public bool Value { get; set; }

        public override string[] InputPortGuids => new string[] { };

        [SerializeField] PortData outputFlowPortData;
        public PortData OutputFlowPortData => outputFlowPortData;
        public override string[] OutputPortGuids => new string[] { outputFlowPortData.PortGuid };

        public override void Initialize(Vector2 position, DialogueTree dialogueTree)
        {
            base.Initialize(position, dialogueTree);
            outputFlowPortData = new(DialogueTree, Direction.Output);

            SaveChanges();
        }

        public override IEnumerable<GVNodeData> GetChildren()
        {
            return new GVNodeData[] { outputFlowPortData.ConnectedNode.Single() };
        }

        public override void Execute() { }
    }

    [CustomGraphViewNode(typeof(BooleanNode))]
    public class CustomBooleanGraphViewNode : GraphViewNode
    {
        public override void OnDrawNodeView(GVNodeData nodeData)
        {
            if(nodeData is BooleanNode booleanNode)
            {
                SerializedObject SO = new(booleanNode);
                mainContainer.Bind(SO);

                var outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
                outputPort.viewDataKey = booleanNode.OutputPortGuids[0];
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

        protected override void OnPortRemoved(Port port)
        {
            base.OnPortRemoved(port);
            Debug.Log("port removed");
        }
    }
}