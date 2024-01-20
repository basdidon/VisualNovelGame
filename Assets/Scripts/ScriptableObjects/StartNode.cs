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

        // Port
        [SerializeField] string outputPortGuid;
        public override string[] OutputPortGuids => new string[] { outputPortGuid };

        public override void Initialize(Vector2 position, DialogueTree dialogueTree)
        {
            base.Initialize(position, dialogueTree);
            outputPortGuid = Guid.NewGuid().ToString();
            //AssetDatabase.SaveAssets();
        }

        public override Node CreateNode()
        {
            var node = base.CreateNode();
            node.capabilities -= Capabilities.Deletable; // by defualt node can be delete,so i remove deletable
            return node;
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

        public override void Draw(Node node)
        {
            node.titleButtonContainer.style.display = DisplayStyle.None;
            // output port
            Port outputPort = node.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            outputPort.viewDataKey = outputPortGuid;
            outputPort.portName = "Output";
            node.outputContainer.Add(outputPort);

            node.RefreshExpandedState();
        }

        public void Execute()
        {
            Debug.Log("StartNode was executed.");

            if (Child is null)
            {
                Debug.LogWarning("StartNode dosen't connect to anything.");
            }
            else 
            { 
                DialogueManager.Instance.CurrentNode = Child;
            }
        }
    }
}