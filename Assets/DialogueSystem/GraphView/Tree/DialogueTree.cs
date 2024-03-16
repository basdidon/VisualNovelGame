using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
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

        // return null if not found
        public BaseNode GetNodeByPort(PortData portData) => GetNodeByPort(portData.PortGuid,portData.Direction);
        public BaseNode GetNodeByPort(string portGuid,Direction direction)
        {
            return direction switch
            {
                Direction.Input => Nodes.SingleOrDefault(n => n.GetPortGuids(Direction.Input).Contains(portGuid)),
                Direction.Output => Nodes.SingleOrDefault(n => n.GetPortGuids(Direction.Output).Contains(portGuid)),
                _ => throw new InvalidOperationException("Invalid port direction.")
            };
        }

        // ConnectedEdges
        public IEnumerable<EdgeData> GetConnectedEdges(PortData portData) => GetConnectedEdges(portData.PortGuid,portData.Direction);
        public IEnumerable<EdgeData> GetConnectedEdges(string portGuid, Direction direction)
        {
            return direction switch
            {
                Direction.Input => Edges.Where(e => e.InputPortGuid == portGuid),
                Direction.Output => Edges.Where(e => e.OutputPortGuid == portGuid),
                _ => throw new InvalidOperationException("Invalid port direction.")
            };
        }

        // ConnectedPorts
        public IEnumerable<string> GetConnectedPortsGuid(string portGuid, Direction direction)
        {
            return direction switch
            {
                Direction.Input => GetConnectedEdges(portGuid, direction).Select(e => e.OutputPortGuid),
                Direction.Output => GetConnectedEdges(portGuid, direction).Select(e => e.InputPortGuid),
                _ => throw new InvalidOperationException("Invalid port direction.")
            };
        }

        public IEnumerable<T> GetConnectedNodes<T>(PortData portData) => GetConnectedNodes(portData).OfType<T>();
        public IEnumerable<BaseNode> GetConnectedNodes(PortData portData)
        {
            if (portData == null)
                throw new System.ArgumentNullException();

            return GetConnectedEdges(portData).Select(e => portData.Direction switch
            {
                Direction.Input => GetNodeByPort(e.OutputPortGuid,Direction.Output),
                Direction.Output => GetNodeByPort(e.InputPortGuid,Direction.Input),
                _ => throw new InvalidOperationException("Invalid port direction.")
            });
        }

        public T GetInputValue<T>(string inputPortGuid, T defaultValue)
        {
            var edges = GetConnectedEdges(inputPortGuid, Direction.Input);

            if (edges.Count() == 0)
                return defaultValue;

            if (edges.Count() != 1)
                throw new Exception("edge more than 1");

            EdgeData edge = edges.First();

            var outputNode = GetNodeByPort(edge.OutputPortGuid, Direction.Output);
            if (outputNode == null)
                throw new Exception($"not found any port match with {edge.OutputPortGuid}");

            var portValue = outputNode.GetValue(edge.OutputPortGuid);
            if (portValue is not T)
                throw new Exception($"port value is not {typeof(T)}");
            

            return (T) portValue;
        }

#if UNITY_EDITOR
        public void ReInitializePorts()
        {
            Nodes.ForEach(node => node.InstantiatePorts());

            edges.Clear();
        }
        public static string GetCurrentProjectBrowserDirectory()
        {
            Type projectWindowUtilType = typeof(ProjectWindowUtil);
            MethodInfo getActiveFolderPath = projectWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
            object obj = getActiveFolderPath.Invoke(null, new object[0]);
            return obj.ToString();
        }

        [MenuItem("H8 Tools/GraphTree/Create")]
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

                string assetPath = AssetDatabase.GetAssetPath(instanceID);

                GraphEditorWindow.OpenWindow(assetPath);

            }

            // Window should now be open, proceed to next step to open file
            return false;
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(DialogueTree))]
    public class DialogueTreeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("re-initialize"))
            {
                (target as DialogueTree).ReInitializePorts();
            }
        }
    }
#endif
}