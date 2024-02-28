using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

namespace BasDidon.Dialogue.VisualGraphView
{
    public class DialogueEditorWindow : EditorWindow
    {
        DialogueGraphView graphView;
        public DialogueGraphView DialogueGraphView
        {
            get => graphView;
            set
            {
                graphView = value;

                DialogueGraphView.StretchToParentSize();
                rootVisualElement.Add(DialogueGraphView);
            }
        }

        string lastOpenPath = string.Empty;


        /*
        [MenuItem("Window/Dialogue/Dialogue GraphView")]
        public static void Open()
        { 
            Debug.Log("Open!!");
            DialogueEditorWindow editorWindow = GetWindow<DialogueEditorWindow>();
            editorWindow.titleContent = new GUIContent("Dialogue Graphview");
        }
        */

        // OnEnable call every scene reload, i just reload graphView to this window
        private void OnEnable()
        {
            if (string.IsNullOrEmpty(lastOpenPath))
                return;
            LoadFileFromPath(lastOpenPath);
        }

        public void LoadFileFromPath(string path)
        {
            try
            {
                DialogueGraphView = new(path);
                lastOpenPath = path;
            }
            catch
            {
                Close();
            }

        }

        public static void OpenWindow(string assetPath)
        {
            try
            {
                DialogueGraphView graphView = new(assetPath);

                var window = GetWindow<DialogueEditorWindow>();
                window.DialogueGraphView = graphView;
            }
            catch
            {
                Debug.LogError("can not open dialogueEditorWindow");
                throw;
            }

        }


        private void OnDestroy()
        {
            if (graphView != null)
            {
                EditorUtility.SetDirty(graphView.Tree);
                AssetDatabase.SaveAssetIfDirty(graphView.Tree);
                Debug.Log("saved on destroy");
            }
        }
    }
}