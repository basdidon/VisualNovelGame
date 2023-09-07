using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

public class DialogueEditorWindow : EditorWindow
{
    [MenuItem("Window/Dialogue/Dialogue GraphView")]
    public static void Open()
    {
        Debug.Log("Open!!");
        DialogueEditorWindow editorWindow = GetWindow<DialogueEditorWindow>();
        editorWindow.titleContent = new GUIContent("Dialogue Graphview");
    }

    private void OnEnable()
    {
        AddGraphview();
    }

    void AddGraphview()
    {
        DialogueGV graphView = new();
        graphView.StretchToParentSize();
        rootVisualElement.Add(graphView);

    }
}
