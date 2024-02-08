using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Reflection;

namespace BasDidon.Dialogue.VisualGraphView
{
    public class DialogueTree : ScriptableObject
    {
        [field: SerializeField] public List<NodeData> Nodes { get; private set; } = new();
        [SerializeField] List<EdgeData> edges = new();
        public IReadOnlyList<EdgeData> Edges => edges;

        public StartNode StartNode => AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this))
                .OfType<StartNode>()
                .First();

        public void AddEdge(EdgeData edgeData)
        {
            edges.Add(edgeData);
            OnAddEdge?.Invoke(edgeData);

            SaveChanges();
        }

        public void RemoveEdge(EdgeData edgeData)
        {
            edges.Remove(edgeData);
            OnRemoveEdge?.Invoke(edgeData);

            SaveChanges();
        }

        public event Action<EdgeData> OnAddEdge;
        public event Action<EdgeData> OnRemoveEdge;

        public void SaveChanges()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
            AssetDatabase.Refresh();
        }

#if UNITY_EDITOR
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

            var uniquePath = AssetDatabase.GenerateUniqueAssetPath($"{currentDirectory}/DialogueTree.asset");
            Debug.Log(uniquePath);

            AssetDatabase.CreateAsset(tree, uniquePath);

            NodeFactory.CreateNode<StartNode>(Vector2.zero, tree);
            
            EditorUtility.SetDirty(tree);
            AssetDatabase.SaveAssetIfDirty(tree);
        }

        [OnOpenAsset(0)]
        public static bool OpenDialogueEditorWindow(int instanceID, int line)
        {
            if (EditorUtility.InstanceIDToObject(instanceID).GetType() == typeof(DialogueTree))
            {
                Debug.Log($"{EditorUtility.InstanceIDToObject(instanceID).GetType()} : {typeof(DialogueTree)}");

                var window = EditorWindow.GetWindow<DialogueEditorWindow>();
                string assetPath = AssetDatabase.GetAssetPath(instanceID);
                window.LoadFileFromPath(assetPath);
            }

            // Window should now be open, proceed to next step to open file
            return false;
        }
#endif
    }
}