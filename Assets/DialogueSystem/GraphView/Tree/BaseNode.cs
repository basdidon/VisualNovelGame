using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using System.Linq;
using System.Reflection;
using UnityEngine.UIElements;

namespace BasDidon.Dialogue.VisualGraphView
{
    [Serializable]
    public class PortCollection: SerializedDictionary<string,PortData>{}

    public abstract class BaseNode : ScriptableObject
    {
        [SerializeField] DialogueTree dialogueTree;
        public DialogueTree DialogueTree 
        {
            get
            {
                if (dialogueTree == null)
                    throw new NullReferenceException($"{GetType().Name} dosen't have dialogueTree");
                return dialogueTree;
            }
            private set
            {
                dialogueTree = value;
            }
        }

        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public Vector2 GraphPosition { get; set; }             // position on graphview

        // Port
        [SerializeField] PortCollection ports;
        public IEnumerable<KeyValuePair<string, PortData>> Ports => ports;
        public IEnumerable<KeyValuePair<string, PortData>> InputPorts => ports.Where(pair => pair.Value?.Direction == Direction.Input);
        public IEnumerable<KeyValuePair<string, PortData>> OutputPorts => ports.Where(pair => pair.Value?.Direction == Direction.Output);
        
        public IEnumerable<string> GetPortGuids() => Ports.Select(pair => pair.Value.PortGuid);
        public IEnumerable<string> GetPortGuids(Direction direction) => Ports.Where(pair => pair.Value.Direction == direction).Select(pair => pair.Value.PortGuid);

        public PortData GetPortData(string key) => ports.GetValueOrDefault(key);

        //

        public virtual void Initialize(Vector2 position, DialogueTree dialogueTree)
        {
            Debug.Log($"{GetType()} initialize");
            Id = $"{Guid.NewGuid()}";
            GraphPosition = position;
            DialogueTree = dialogueTree;
            name = GetType().Name;

            InstantiatePorts();

            dialogueTree.Nodes.Add(this);
            AssetDatabase.AddObjectToAsset(this, dialogueTree);

            SaveChanges();
        }

        public void InstantiatePorts()
        {
            Debug.Log($"{GetType()} InstantiatePorts()");
            ports = new();

            var members = GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            
            foreach (var member in members)
            {
                PortData newPortData = null;

                if(member.IsDefined(typeof(InputAttribute), inherit: true))
                {
                    newPortData = new(Direction.Input,typeof(ExecutionFlow));
                }

                if(member.IsDefined(typeof(OutputAttribute), inherit: true))
                {
                    newPortData = new(Direction.Output,typeof(ExecutionFlow));
                    Debug.Log(newPortData.Type);
                }

                if (newPortData == null)
                    continue;

                ports.Add(member.Name, newPortData);
                Debug.Log($"added new {newPortData.Direction} Port : {member.Name} {newPortData.PortGuid}");
            }
        }

        public virtual object GetValue(string outputPortGuid) => throw new NotImplementedException();

        public void SaveChanges()
        {
            // save node asset
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
        }
    }

    public abstract class ExecutableNode : BaseNode, IExecutableNode
    {
        [Input] public ExecutionFlow input;
        [Output] public ExecutionFlow output;

        public abstract void OnEnter();
        public abstract void OnExit();
    }

    public interface IExecutableNode
    {
        public void OnEnter();
        public void OnExit();
    }

    [SerializeField]
    [AttributeUsage(AttributeTargets.Field)]
    public class InputAttribute : Attribute{}

    [SerializeField]
    [AttributeUsage(AttributeTargets.Field)]
    public class OutputAttribute : Attribute{}

    [SerializeField]
    [AttributeUsage(AttributeTargets.Field)]
    public class NodeFieldAttribute :Attribute{}

    [AttributeUsage(AttributeTargets.Class)]
    public class CreateNodeMenuAttribute: Attribute
    {
        public string menuName;
        
        static readonly Assembly[] assemblyToSearch = {
            Assembly.GetExecutingAssembly(),    // this assembly
            Assembly.Load("Assembly-CSharp"),   // default assembly in Assets/Scripts
        };

        public static IEnumerable<BaseNode> GetAllBaseNode()
        {
            foreach (var assembly in assemblyToSearch)
            {
                var typesWithAttribute = assembly.GetTypes()
                    .Where(type => type.IsSubclassOf(typeof(BaseNode)) && GetCustomAttribute(type, typeof(CreateNodeMenuAttribute)) != null);

                foreach (var type in typesWithAttribute)
                {
                    var instance = ScriptableObject.CreateInstance(type) as BaseNode;
                    if(instance != null)
                        yield return instance;
                }
            }
        }

        public static void PopulateCreateContextualMenu(DialogueGraphView graphView)
        {
            foreach (var assembly in assemblyToSearch)
            {
                var typeMenuNamePair = assembly.GetTypes()
                    .Where(type => type.IsSubclassOf(typeof(BaseNode)) && GetCustomAttribute(type, typeof(CreateNodeMenuAttribute)) != null)
                    .Select(type=> new { Type = type, (GetCustomAttribute(type,typeof(CreateNodeMenuAttribute)) as CreateNodeMenuAttribute).menuName });

                foreach (var pair in typeMenuNamePair)
                {
                    graphView.AddManipulator(new ContextualMenuManipulator(ev=> { 
                        ev.menu.AppendAction(pair.menuName, actionEvent => {
                            // create baseNode
                            BaseNode baseNode = NodeFactory.CreateNode(pair.Type, actionEvent.eventInfo.localMousePosition, graphView.Tree);
                            // get nodeView by baseNode
                            GraphViewNode nodeView = NodeFactory.GetNodeView(baseNode, graphView);
                            // add nodeView to graph
                            graphView.AddElement(nodeView);
                        });
                    }));
                }
            }
        }
    }
}