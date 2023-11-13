using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor;

public abstract class GVNodeData : ScriptableObject
{
    [field: SerializeField] public string Id { get; private set; }
    [field: SerializeField] public Vector2 GraphPosition { get; set; }             // position on graphview

    // Port
    public abstract string InputPortGuid { get; }
    public abstract string[] OutputPortGuids { get; }

    // Ui Controller
    public UiDocumentController UDC => UiDocumentController.Instance;
    public DialogueUiController DialogueUC => UDC.DialogueUiController;
    public ChoicesPickerUiController ChoicesPickerUC => UDC.ChoicesPickerUiController;

    public virtual void Initialize(Vector2 position,DialogueTree dialogueTree)
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
        var node = Activator.CreateInstance<GraphViewNode>();
        node.Initialize(this);

        Draw(node);

        return node;
    }

    public abstract void Draw(Node node);

    public abstract void AddChild(GVNodeData child);
    public abstract void RemoveChild(GVNodeData child);
    public abstract IEnumerable<GVNodeData> GetChildren();
    public abstract void Execute();
    public abstract void Next();
}