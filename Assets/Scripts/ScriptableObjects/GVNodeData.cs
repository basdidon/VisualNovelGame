using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor;

public abstract class GVNodeData : ScriptableObject
{
    [field: SerializeField] public string Id { get; private set; }
    [field: SerializeField] public Vector2 GraphPosition { get; set; }             // position on graphview

    public virtual void Initialize(Vector2 position,DialogueTree dialogueTree)
    {
        Id = $"{Guid.NewGuid()}";
        GraphPosition = position;
        name = GetType().Name;
        // save node asset
        AssetDatabase.AddObjectToAsset(this, dialogueTree);
        AssetDatabase.SaveAssets();
        dialogueTree.Dialogues.Add(this);
    }

    public virtual Node CreateNode()
    {
        var node = Activator.CreateInstance<GraphViewNode>();
        node.Initialize(this);

        Draw(node);

        return node;
    }

    public abstract void Draw(Node node);

    public abstract void AddChild(GVNodeData child);
    public abstract void RemoveChild(GVNodeData child);
    public abstract IEnumerable<GVNodeData> GetChildren();

    // Port



}