using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Reflection;
using UnityEditor.Experimental.GraphView;

namespace BasDidon.Dialogue.VisualGraphView
{
    public class DialogueTree : ScriptableObject
    {
        [field: SerializeField] public List<BaseNode> Nodes { get; private set; } = new();
        [SerializeField] List<EdgeData> edges = new();
        public IReadOnlyList<EdgeData> Edges => edges;

        public StartNode StartNode => AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this))
                .OfType<StartNode>()
                .First();

        public void AddEdge(EdgeData edgeData)
        {
            edges.Add(edgeData);

            SaveChanges();
        }

        public void RemoveEdge(EdgeData edgeData)
        {
            edges.Remove(edgeData);

            SaveChanges();
        }

        public void SaveChanges()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
            AssetDatabase.Refresh();
        }

        public IEnumerable<EdgeData> GetConnectedEdges(PortData portData)
        {
            return portData.Direction switch
            {
                Direction.Input => Edges.Where(e => e.InputPortGuid == portData.PortGuid),
                Direction.Output => Edges.Where(e => e.OutputPortGuid == portData.PortGuid),
                _ => throw new InvalidOperationException("Invalid port direction.")
            };
        }

        public BaseNode GetNodeByPort(PortData portData)
        {
            return GetNodeByPort(portData.PortGuid,portData.Direction);
        }

        public BaseNode GetNodeByPort(string portGuid,Direction direction)
        {
            return direction switch
            {
                Direction.Input => Nodes.Single(n => n.GetPortGuids(Direction.Input).Contains(portGuid)),
                Direction.Output => Nodes.Single(n => n.GetPortGuids(Direction.Output).Contains(portGuid)),
                _ => throw new InvalidOperationException("Invalid port direction.")
            };
        }

        public IEnumerable<BaseNode> GetConnectedNodes(PortData portData)
        {
            return GetConnectedEdges(portData).Select(e => portData.Direction switch
            {
                Direction.Input => GetNodeByPort(e.OutputPortGuid,Direction.Output),
                Direction.Output => GetNodeByPort(e.InputPortGuid,Direction.Input),
                _ => throw new InvalidOperationException("Invalid port direction.")
            });
        }

        public IEnumerable<T> GetConnectedNodes<T>(PortData portData)
        {
            return GetConnectedNodes(portData).OfType<T>();
        }

        public object GetData(string inputPortGuid)
        {
            var edge = Edges.SingleOrDefault(e => e.InputPortGuid == inputPortGuid);

            if (edge == null)
            {
                Debug.Log("port is not connect. or have a port with same guid");
                return default;
            }

            var outputNode = Nodes.SingleOrDefault(n => n.GetPortGuids(Direction.Output).Contains(edge.OutputPortGuid));

            if (outputNode == null)
            {
                Debug.Log($"not found any port match with {edge.OutputPortGuid}");
                throw new Exception();
            }

            return outputNode.ReadValueFromPort(edge.OutputPortGuid);
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