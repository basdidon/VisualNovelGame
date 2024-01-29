using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;

namespace Graphview.NodeData
{
    using NodeView;
    using System;
    using UnityEditor;

    public class BooleanNode : GVNodeData
    {
        [SerializeField] bool value;
        [field:SerializeField] public bool Value { get; set; }

        [field: SerializeField] GVNodeData Child { get; set; }

        public override string[] InputPortGuids => new string[] { };

        [SerializeField] string outputPortGuid;
        public override string[] OutputPortGuids => new string[] { outputPortGuid };

        public override void Initialize(Vector2 position, DialogueTree dialogueTree)
        {
            base.Initialize(position, dialogueTree);
            outputPortGuid = Guid.NewGuid().ToString();
            Debug.Log($"output guid : {outputPortGuid}");
            SaveChanges();
        }

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