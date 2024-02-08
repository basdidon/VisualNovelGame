using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

namespace BasDidon.Dialogue.VisualGraphView
{
    public class DialogueEditorWindow : EditorWindow
    {
        DialogueGraphView graphView;
        /*
        [MenuItem("Window/Dialogue/Dialogue GraphView")]
        public static void Open()
        { 
            Debug.Log("Open!!");
            DialogueEditorWindow editorWindow = GetWindow<DialogueEditorWindow>();
            editorWindow.titleContent = new GUIContent("Dialogue Graphview");
        }
        */
        public void LoadFileFromPath(string path)
        {
            graphView = new(path);
            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);
        }

        private void OnDestroy()
        {
            if (graphView != null)
            {
                EditorUtility.SetDirty(graphView.Tree);
                AssetDatabase.SaveAssetIfDirty(graphView.Tree);
                Debug.Log("saved");
            }
        }
    }
}