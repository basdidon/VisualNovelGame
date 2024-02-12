using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using System.Linq;

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

        protected PortData InstantiatePortData(Direction direction)
        {
            PortData newPortData = new(direction);
            return newPortData;
        }

        public virtual void Initialize(Vector2 position, DialogueTree dialogueTree)
        {
            Id = $"{Guid.NewGuid()}";
            GraphPosition = position;
            DialogueTree = dialogueTree;
            name = GetType().Name;

            ports = new();

            InstantiatePorts();

            dialogueTree.Nodes.Add(this);
            AssetDatabase.AddObjectToAsset(this, dialogueTree);

            SaveChanges();
        }

        protected void InstantiatePorts()
        {
            var inputProperties = GetType().GetProperties().Where(p=>p.IsDefined(typeof(InputAttribute),inherit: true));
            var outputProperties = GetType().GetProperties().Where(p => p.IsDefined(typeof(OutputAttribute), inherit: true));
            
            foreach (var inputProperty in inputProperties)
            {
                PortData newPortData = new(Direction.Input);
                ports.Add(inputProperty.Name, newPortData);
                Debug.Log($"{GetType()} add inputPortData : {inputProperty.Name} {newPortData.PortGuid}");
            }

            foreach (var outputProperty in outputProperties)
            {
                PortData newPortData = new(Direction.Output);
                ports.Add(outputProperty.Name, newPortData);
                Debug.Log($"{GetType().Name} add inputPortData : {outputProperty.Name}, {newPortData.PortGuid}");
            }
        }

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

    [SerializeField]
    public class PortAttribute : Attribute
    {
        public string PortGuid { get; }

        public PortAttribute()
        {
            PortGuid = Guid.NewGuid().ToString();
        }
    }

    [SerializeField]
    public class InputAttribute : PortAttribute{}

    [SerializeField]
    public class OutputAttribute : PortAttribute{}

    [SerializeField]
    public class NodeFieldAttribute :Attribute{}
}