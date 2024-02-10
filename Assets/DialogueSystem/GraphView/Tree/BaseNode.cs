using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using System.Linq;

namespace BasDidon.Dialogue.VisualGraphView
{
    public abstract class BaseNode : ScriptableObject
    {
        DialogueTree dialogueTree;
        public DialogueTree DialogueTree
        {
            get
            {
                if (dialogueTree == null)
                    throw new NullReferenceException();
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
        [SerializeField] List<PortData> PortsData;
        public IEnumerable<string> InputPortGuids => PortsData.Where(p => p.Direction == Direction.Input).Select(p => p.PortGuid);
        public IEnumerable<string> OutputPortGuids => PortsData.Where(p => p.Direction == Direction.Output).Select(p => p.PortGuid);
        
        protected PortData InstantiatePortData(Direction direction)
        {
            PortData newPortData = new(DialogueTree, direction);
            PortsData.Add(newPortData);
            return newPortData;
        }

        public virtual void Initialize(Vector2 position, DialogueTree dialogueTree)
        {
            Id = $"{Guid.NewGuid()}";
            GraphPosition = position;
            DialogueTree = dialogueTree;
            name = GetType().Name;

            PortsData = new();

            OnInstantiatePortData();
            
            // InstantiatePorts();

            dialogueTree.Nodes.Add(this);
            AssetDatabase.AddObjectToAsset(this, dialogueTree);

            SaveChanges();
        }
        /*
        private void InstantiatePorts()
        {
            if (GetType().IsDefined(typeof(ExecutableNodeAttribute), inherit: true))
            {
                ExecutableNodeAttribute executable = (ExecutableNodeAttribute) Attribute.GetCustomAttribute(GetType(),typeof(ExecutableNodeAttribute), true);
                Debug.Log($"{GetType() } defined ExecutableNode : {executable.HasInputPort}, {executable.HasOutputPort}");
                
                if (executable.HasInputPort) 
                    executable.InputPort = InstantiatePortData(Direction.Input);
                if (executable.HasOutputPort)
                    executable.OutputPort = InstantiatePortData(Direction.Output);
            }
            else
            {
                Debug.Log($"{GetType() } not define ExecutableNode");
                OnInstantiatePortData();
            }
        }*/

        public abstract void OnInstantiatePortData();

        public virtual object ReadValueFromPort(string outputPortGuid) => throw new NotImplementedException();

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

    public abstract class ExecutableNode : BaseNode,IExecutableNode
    {
        public string InputPortGuid;
        public string OutputPortGuid;

        public override void Initialize(Vector2 position, DialogueTree dialogueTree)
        {
            base.Initialize(position, dialogueTree);
        }

        public abstract void OnEnter();

        public abstract void OnExit();

        public abstract override void OnInstantiatePortData();
    }
    /*
[AttributeUsage(AttributeTargets.Class)]
public class ExecutableNodeAttribute : Attribute
{
    public bool HasInputPort { get; }
    public bool HasOutputPort { get; }

    public string InputPortGuid { get; }
    public string OutputPortGuid { get; }

    public ExecutableNodeAttribute(bool hasInputPort = true, bool hasOutputPort = true)
    {
        HasInputPort = hasInputPort;
        HasOutputPort = hasOutputPort;

        InputPortGuid = Guid.NewGuid().ToString();
        OutputPortGuid = Guid.NewGuid().ToString();
    }

    public static IEnumerable<Type> GetTypes()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsDefined(typeof(ExecutableNodeAttribute), inherit: true))
                {
                    yield return type;
                }
            }
        }
    }
}*/

    [SerializeField]
    public class PortAttribute : Attribute
    {
        public string PortGuid { get; }

        public PortAttribute()
        {
            PortGuid = Guid.NewGuid().ToString();
        }
    }

    public class InputAttribute : PortAttribute
    {
    }

    public class OutputAttribute : PortAttribute
    {

    }
}