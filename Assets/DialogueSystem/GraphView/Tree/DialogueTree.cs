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
            return portData.Direction switch
            {
                Direction.Input => Nodes.Single(n=> n.InputPortGuids.Contains(portData.PortGuid)),
                Direction.Output => Nodes.Single(n=> n.OutputPortGuids.Contains(portData.PortGuid)),
                _ => throw new InvalidOperationException("Invalid port direction.")
            };
        }

        public IEnumerable<BaseNode> GetConnectedNodes(PortData portData)
        {
            throw new NotImplementedException();
        }

        public object GetData(string inputPortGuid)
        {
            if (!Edges.Any(e => e.InputPortGuid == inputPortGuid))
            {
                Debug.Log("port is not connect.");
                return null;
            }

            var edge = Edges.SingleOrDefault(e => e.InputPortGuid == inputPortGuid);

            if (edge == null)
                return default;

            var outputNode = Nodes.SingleOrDefault(n => n.OutputPortGuids.Contains(edge.OutputPortGuid));

            if (outputNode == null)
                throw new Exception();

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