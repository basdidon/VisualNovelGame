using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;

[CreateAssetMenu(menuName = "DialoguesTree")]
public class DialogueTree: ScriptableObject
{
    public List<GVNodeData> Nodes = new();
    public List<EdgeData> Edges = new();

    [OnOpenAsset(1)]
    public static bool OpenDialogueEditorWindow(int instanceID, int line)
    {
        bool windowIsOpen = EditorWindow.HasOpenInstances<DialogueEditorWindow>();
        if (!windowIsOpen)
            EditorWindow.CreateWindow<DialogueEditorWindow>();
        else
            EditorWindow.FocusWindowIfItsOpen<DialogueEditorWindow>();

        // Window should now be open, proceed to next step to open file
        return false;
    }

    [OnOpenAsset(2)]
    public static bool OpenDialogueGraphView(int instanceID, int line)
    {
        var window = EditorWindow.GetWindow<DialogueEditorWindow>();

        string assetPath = AssetDatabase.GetAssetPath(instanceID);
        window.LoadFromFile(assetPath);

        return true;
    }
}

[Serializable]
public class EdgeData
{
    [field: SerializeField] public string From { get; private set; }
    [field: SerializeField] public string To { get; private set; }
    [field: SerializeField] public string Id { get; private set; }

    public EdgeData(string from,string to,string id)
    {
        From = from;
        To = to;
        Id = id;
    }
}