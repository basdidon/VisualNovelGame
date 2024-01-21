using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor;

namespace Graphview.NodeData
{
    public abstract class GVNodeData : ScriptableObject
    {
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public Vector2 GraphPosition { get; set; }             // position on graphview

        // Port
        public string InputPortGuid => Id;
        public abstract string[] OutputPortGuids { get; }

        public virtual void Initialize(Vector2 position, DialogueTree dialogueTree)
        {
            Id = $"{Guid.NewGuid()}";
            GraphPosition = position;
            name = GetType().Name;
            dialogueTree.Nodes.Add(this);
            // save node asset
            AssetDatabase.AddObjectToAsset(this, dialogueTree);
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
        }

        public virtual Node CreateNode()
        {
            var nodeView = Activator.CreateInstance<GraphViewNode>();
            nodeView.Initialize(this);

            Draw(nodeView);

            return nodeView;
        }

        public abstract void Draw(Node node);

        public abstract void AddChild(GVNodeData child);
        public abstract void RemoveChild(GVNodeData child);
        public abstract IEnumerable<GVNodeData> GetChildren();

        public virtual void DrawInputPort(Node node)
        {
            // input port
            Port inputPort = node.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            inputPort.viewDataKey = InputPortGuid;
            inputPort.portName = "input";
            node.inputContainer.Add(inputPort);
        }
    }
}