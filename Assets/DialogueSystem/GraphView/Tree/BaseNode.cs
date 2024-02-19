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
    public class PortCollection : SerializedDictionary<string, PortData> { }

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
            Debug.Log($"{GetType().Name} InstantiatePorts()");
            ports = new();

            var members = GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var member in members)
            {
                if (member.IsDefined(typeof(PortAttribute), inherit: true))
                {
                    PortAttribute portAttr = member.GetCustomAttribute<PortAttribute>();
                    string bindingPath = PortAttribute.GetBindingPath(member);
                    Type portType = PortAttribute.GetTypeOfMember(member);

                    PortData newPortData = new(portAttr.Direction, portType);

                    ports.Add(bindingPath, newPortData);
                    Debug.Log($"added new {newPortData.Direction} Port : {member.Name} {newPortData.PortGuid}");
                }
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

    public interface IExecutableNode
    {
        public void OnEnter();
        public void OnExit();
    }

    public enum PortDirection
    {
        Input,
        Output
    }

    [SerializeField]
    [AttributeUsage(AttributeTargets.Field|AttributeTargets.Property)]
    public class PortAttribute : Attribute
    {
        public Direction Direction { get; }
        
        public PortAttribute(PortDirection direction)
        {
            Direction = direction switch
            {
                PortDirection.Input => Direction.Input,
                PortDirection.Output => Direction.Output,
                _ => throw new InvalidOperationException()
            };
        }
        public static Type GetTypeOfMember(MemberInfo member)
        {
            if (!member.IsDefined(typeof(PortAttribute), inherit: true))
                throw new Exception("The member is not decorated with the PortAttribute.");

            Debug.Log($"{member.DeclaringType.Name} {member.Name} ({member.MemberType})");

            if (member.MemberType == MemberTypes.Property)
            {
                return (member as PropertyInfo).PropertyType;
            }
            else if (member.MemberType == MemberTypes.Field)
            {
                if (member.Name.Contains(">k__BackingField"))
                {
                    var propertyName = member.Name.Replace("<", "").Replace(">k__BackingField", "");
                    var property = member.DeclaringType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    return property.PropertyType;
                }
                else
                {
                    return (member as FieldInfo).FieldType;
                }
            }
            else
            {
                throw new InvalidOperationException("Unsupported member type.");
            }
        }

        public static string GetBindingPath(MemberInfo member)
        {
            if (!member.IsDefined(typeof(PortAttribute), inherit: true))
                throw new Exception("The member is not decorated with the PortAttribute.");

            return member.MemberType switch
            {
                MemberTypes.Property => $"<{member.Name}>k__BackingField",
                MemberTypes.Field => member.Name,
                _ => throw new InvalidOperationException("Unsupported member type.")
            };
        }
    }

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

        [Obsolete]
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
                            NodeView nodeView = NodeFactory.GetNodeView(baseNode, graphView);
                            // add nodeView to graph
                            graphView.AddElement(nodeView);
                        });
                    }));
                }
            }
        }
    }
}