using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;

namespace Graphview.NodeData
{
    public abstract class GVNodeData : ScriptableObject
    {
        public DialogueTree DialogueTree { get; private set; }

        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public Vector2 GraphPosition { get; set; }             // position on graphview

        // Port
        public abstract string[] InputPortGuids { get; }
        public abstract string[] OutputPortGuids { get; }

        public virtual void Initialize(Vector2 position, DialogueTree dialogueTree)
        {
            Id = $"{Guid.NewGuid()}";
            GraphPosition = position;
            DialogueTree = dialogueTree;
            name = GetType().Name;
            dialogueTree.Nodes.Add(this);
            AssetDatabase.AddObjectToAsset(this, dialogueTree);
            SaveChanges();
        }

        public abstract void AddChild(GVNodeData child);
        public abstract void RemoveChild(GVNodeData child);
        public abstract IEnumerable<GVNodeData> GetChildren();

        public abstract void Execute();

        public void SaveChanges()
        {
            // save node asset
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
        }
    }
}