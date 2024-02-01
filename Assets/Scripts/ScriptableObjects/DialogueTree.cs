using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Reflection;
using UnityEditor.Experimental.GraphView;

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
        [field: SerializeField] public DialogueTree DialogueTree { get; private set; }
        [field: SerializeField] public string EdgeGuid { get; private set; }
        [field: SerializeField] public string OutputPortGuid { get; private set; }
        [field: SerializeField] public string InputPortGuid { get; private set; }

        public EdgeData(DialogueTree dialogueTree, string outputPortGuid, string inputPortGuid)
        {
            DialogueTree = dialogueTree;
            EdgeGuid = Guid.NewGuid().ToString();   // generate guid
            OutputPortGuid = outputPortGuid;
            InputPortGuid = inputPortGuid;
        }

        public static EdgeData GetEdgeData(DialogueTree dialogueTree,string edgeGuid)=> dialogueTree.Edges.First(e => e.EdgeGuid == edgeGuid);
        
        public GVNodeData GetInputNodeData()=> DialogueTree.Nodes.First(n=>n.InputPortGuids.Contains(InputPortGuid));
        public GVNodeData GetOutputNodeData()=> DialogueTree.Nodes.First(n=>n.OutputPortGuids.Contains(OutputPortGuid));
    }

    [Serializable]
    public class PortData
    {
        DialogueTree DialogueTree { get; set; }

        [field: SerializeField] public string PortGuid { get; private set; }
        [field: SerializeField] public Direction Direction { get; private set; }
        [field: SerializeField] public List<EdgeData> EdgesData { get; private set; }

        [field: SerializeField] public List<GVNodeData> ConnectedNode { get; private set; }

        public PortData(DialogueTree dialogueTree,Direction direction)
        {
            DialogueTree = dialogueTree;
            Direction = direction;
        
            PortGuid = Guid.NewGuid().ToString();
            EdgesData = new();
            ConnectedNode = new();

            UpdatePortData();

            DialogueTree.OnAddEdge += OnAddEdge;
            DialogueTree.OnRemoveEdge += OnRemoveEdge;

            Debug.Log($"create port {PortGuid}");
        }

        void OnAddEdge(EdgeData edgeData)
        {
            if(Direction == Direction.Input && edgeData.InputPortGuid == PortGuid)
            {
                EdgesData.Add(edgeData);
            }
            if(Direction == Direction.Output && edgeData.OutputPortGuid == PortGuid)
            {
                EdgesData.Add(edgeData);
            }

            UpdateConnectedNode();
        }

        void OnRemoveEdge(EdgeData edgeData)
        {
            if (Direction == Direction.Input && edgeData.InputPortGuid == PortGuid)
            {
                EdgesData.Remove(edgeData);
            }
            if (Direction == Direction.Output && edgeData.OutputPortGuid == PortGuid)
            {
                EdgesData.Remove(edgeData);
            }

            UpdateConnectedNode();
        }

        public void UpdatePortData()
        {
            if (Direction == Direction.Input)
            {
                EdgesData = DialogueTree.Edges.Where(e => e.InputPortGuid == PortGuid).ToList();
            }
            else if (Direction == Direction.Output)
            {
                EdgesData = DialogueTree.Edges.Where(e => e.OutputPortGuid == PortGuid).ToList();
            }

            UpdateConnectedNode();
        }

        void UpdateConnectedNode()
        {
            if(Direction == Direction.Input)
            {
                ConnectedNode = EdgesData.Select(e => e.GetOutputNodeData()).ToList();
            }
            else if(Direction == Direction.Output)
            {
                ConnectedNode = EdgesData.Select(e => e.GetInputNodeData()).ToList();
                Debug.Log($"{PortGuid} : {ConnectedNode.Count}");
            }
        }
    }
}