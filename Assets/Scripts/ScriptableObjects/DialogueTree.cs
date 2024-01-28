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
            Debug.Log($"add edge {edge.OutputPortGuid} -> {edge.InputPortGuid}");

            var fromNode = Nodes.FirstOrDefault(node => node.OutputPortGuids.Contains(edge.OutputPortGuid));
            var toNode = Nodes.FirstOrDefault(node => node.InputPortGuids.Contains(edge.InputPortGuid));

            if (fromNode == null)
                throw new Exception($"can't find output node ({edge.OutputPortGuid})");

            if (toNode == null)
                throw new Exception($"can't find input node ({edge.InputPortGuid})");

            Debug.Log($"created edge {fromNode.Id} -> {toNode.Id}");
            edges.Add(edge);

            if (fromNode is ChoicesNode choicesNode)
            {
                choicesNode.Connect(edge.OutputPortGuid, toNode);
            }

            fromNode.AddChild(toNode);


            SaveChanges();
        }

        public void RemoveEdge(EdgeData edge)
        {
            var fromNode = Nodes.FirstOrDefault(node => node.OutputPortGuids.Contains(edge.OutputPortGuid));
            var toNode = Nodes.FirstOrDefault(node => node.InputPortGuids.Contains(edge.InputPortGuid));

            if (fromNode == null)
                throw new Exception($"can't find output node ({edge.OutputPortGuid})");

            if (toNode == null)
                throw new Exception($"can't find input node ({edge.InputPortGuid})");

            edges.Remove(edge);

            if (fromNode is ChoicesNode choicesNode)
            {
                choicesNode.Disconnect(edge.OutputPortGuid);
            }

            fromNode.RemoveChild(toNode);


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
        [field: SerializeField] public string EdgeGuid { get; private set; }
        [field: SerializeField] public string OutputPortGuid { get; private set; }
        [field: SerializeField] public string InputPortGuid { get; private set; }

        public EdgeData(string outputPortGuid, string inputPortGuid)
        {
            EdgeGuid = Guid.NewGuid().ToString();   // generate guid
            OutputPortGuid = outputPortGuid;
            InputPortGuid = inputPortGuid;
        }
    }
}