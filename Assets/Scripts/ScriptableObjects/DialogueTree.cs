using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Reflection;

namespace Graphview.NodeData
{
    using NodeView;

    public class DialogueTree : ScriptableObject
    {
        [field: SerializeField] public List<GVNodeData> Nodes { get; private set; } = new();
        [SerializeField] List<EdgeData> edges = new();
        public IReadOnlyList<EdgeData> Edges => edges;

        public StartNode StartNode => AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this))
                .OfType<StartNode>()
                .First();

        public void AddEdge(EdgeData edge)
        {
            Debug.Log("add edge 1");

            var fromIdx = Nodes.FindIndex(node => node.OutputPortGuids.Contains(edge.From));
            var toIdx = Nodes.FindIndex(node => node.InputPortGuid == edge.To);

            Debug.Log($"{fromIdx} -> {toIdx}");

            if (fromIdx >= 0 && toIdx >= 0)
            {

                Debug.Log("add edge 2");
                edges.Add(edge);

                if (Nodes[fromIdx] is ChoicesNode choicesNode)
                {
                    choicesNode.Connect(edge.From, Nodes[toIdx]);
                }

                Nodes[fromIdx].AddChild(Nodes[toIdx]);
            }

            SaveChanges();
        }

        public void RemoveEdge(EdgeData edge)
        {
            var fromIdx = Nodes.FindIndex(node => node.OutputPortGuids.Contains(edge.From));
            var toIdx = Nodes.FindIndex(node => node.InputPortGuid == edge.To);

            if (fromIdx >= 0 && toIdx >= 0)
            {
                edges.Remove(edge);

                if (Nodes[fromIdx] is ChoicesNode choicesNode)
                {
                    choicesNode.Disconnect(edge.From);
                }

                Nodes[fromIdx].RemoveChild(Nodes[toIdx]);
            }

            SaveChanges();
        }

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
            EditorUtility.SetDirty(tree);
            AssetDatabase.SaveAssetIfDirty(tree);
            NodeFactory.CreateNode<StartNode>(Vector2.zero, tree);
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

    [Serializable]
    public class EdgeData
    {
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public string From { get; private set; }
        [field: SerializeField] public string To { get; private set; }

        public EdgeData(string from, string to, string id)
        {
            From = from;
            To = to;
            Id = id;
        }
    }
}