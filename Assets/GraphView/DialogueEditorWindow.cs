using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

public class DialogueEditorWindow : EditorWindow
{
    DialogueGV graphView;
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
        graphView = new();
        graphView.StretchToParentSize();
        rootVisualElement.Add(graphView);
    }
    /*
    private void OnSelectionChange()
    {
        DialogueTree tree = Selection.activeObject as DialogueTree;
        if(tree != null)
        {
            SerializedObject so = new(tree);
            rootVisualElement.Bind(so);
            
        }
    }*/
}
