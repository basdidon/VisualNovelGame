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

    [SerializeField]
    [AttributeUsage(AttributeTargets.Field)]
    public class NodeFieldAttribute :Attribute{}

}