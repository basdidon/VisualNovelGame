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

        public void AddEdge(Edge edge)
        {
            EdgeData edgeData = new(this, edge.output.viewDataKey, edge.input.viewDataKey);
            edge.viewDataKey = edgeData.EdgeGuid;
            
            Debug.Log($"add edge {edgeData.OutputPortGuid} -> {edgeData.InputPortGuid}");

            var fromNode = edgeData.GetOutputNodeData();
            var toNode = edgeData.GetInputNodeData();

            Debug.Log($"created edge ({edgeData.EdgeGuid}) \r\n" +
                $"[Output] node : {fromNode.Id} ,port : {edgeData.OutputPortGuid}\r\n"+
                $"[Input]  node : {toNode.Id} ,port : {edgeData.InputPortGuid}");
            
            edges.Add(edgeData);

            fromNode.AddChild(toNode);

            OnAddEdge?.Invoke(edgeData);

            SaveChanges();
        }

        public void RemoveEdge(EdgeData edgeData)
        {
            var fromNode = edgeData.GetOutputNodeData();
            var toNode = edgeData.GetInputNodeData();

            edges.Remove(edgeData);

            fromNode.RemoveChild(toNode);

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
        [field: SerializeField] public string PortGuid { get; private set; }
        [field: SerializeField] public Direction Direction { get; private set; }
        [field: SerializeField] public Port.Capacity Capacity {get; private set;}

        [field: SerializeField] public EdgeData EdgeData { get; private set; }

        DialogueTree DialogueTree { get; set; }

        public PortData(DialogueTree dialogueTree,Direction direction,Port.Capacity capacity ,EdgeData edgeData)
        {
            DialogueTree = dialogueTree;
            PortGuid = Guid.NewGuid().ToString();
            Direction = direction;
            Capacity = capacity;
            EdgeData = edgeData;
        }

        void OnAddEdge(EdgeData edgeData)
        {
            
        }
    }
}