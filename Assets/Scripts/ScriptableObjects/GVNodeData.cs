using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;

namespace Graphview.NodeData
{
    using NodeView;

    public abstract class GVNodeData : ScriptableObject
    {
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public Vector2 GraphPosition { get; set; }             // position on graphview

        // Port
        public abstract string[] InputPortGuids { get; }
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

        public GraphViewNode GetNodeView()
        {
            return NodeFactory.GetNodeView(this);
        }

        public abstract void AddChild(GVNodeData child);
        public abstract void RemoveChild(GVNodeData child);
        public abstract IEnumerable<GVNodeData> GetChildren();

        public abstract void Execute();
    }
}