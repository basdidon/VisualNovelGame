using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using System.Reflection;

public class DialogueTree: ScriptableObject
{
    [field: SerializeField] public List<GVNodeData> Nodes { get; private set; } = new();
    [field: SerializeField] public List<EdgeData> Edges { get; private set; } = new();

    public void Execute()
    {
        var startNode = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this))
            .OfType<StartNode>()
            .First();
        startNode.Execute();
    }

    public static string GetCurrentProjectBrowserDirectory()
    {
        Type projectWindowUtilType = typeof(ProjectWindowUtil);
        MethodInfo getActiveFolderPath = projectWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
        object obj = getActiveFolderPath.Invoke(null, new object[0]);
        return obj.ToString();
    }

    [MenuItem("MyMenu/CreateDialogueTree")]
    public static void CreateDialogueTree()
    {
        DialogueTree tree = CreateInstance<DialogueTree>();
        var currentDirectory = GetCurrentProjectBrowserDirectory();
        Debug.Log(AssetDatabase.GenerateUniqueAssetPath($"{currentDirectory}/DialogueTree.asset"));
        AssetDatabase.CreateAsset(tree, AssetDatabase.GenerateUniqueAssetPath($"{currentDirectory}/DialogueTree.asset"));
        EditorUtility.SetDirty(tree);
        AssetDatabase.SaveAssets();
        NodeFactory.CreateNode<StartNode>(Vector2.zero,tree);
    }

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