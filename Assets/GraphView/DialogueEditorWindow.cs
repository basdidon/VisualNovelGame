using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

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

    public void LoadFromFile(string path)
    {
        graphView = new(path);
        graphView.StretchToParentSize();
        rootVisualElement.Add(graphView);
    }
}
