using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

namespace BasDidon.Dialogue.VisualGraphView
{
    public class GraphEditorWindow : EditorWindow
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

        // OnEnable call every scene reload, i just reload graphView to this window
        private void OnEnable()
        {
            if (string.IsNullOrEmpty(lastOpenPath))
            {
                return;
            }
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

                var window = GetWindow<GraphEditorWindow>();
                window.lastOpenPath = assetPath;
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