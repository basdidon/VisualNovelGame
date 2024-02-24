using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using System.Linq;
using System.Reflection;
using UnityEngine.UIElements;
using System.Collections;

namespace BasDidon.Dialogue.VisualGraphView
{
    [Serializable]
    public class PortDataCollection : List<PortData>, IList<PortData>,ISerializationCallbackReceiver
    {
        [SerializeField] List<PortData> portList = new();

        #region ISerializationCallbackReceiver
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Clear();
            for (int i = 0; i < portList.Count; i++)
            {
                Add(portList[i]);
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            portList.Clear();

            foreach (var item in this)
            {
                portList.Add(item);
            }
        }
        #endregion

        public IEnumerable<PortData> Ports => portList.AsEnumerable();
        public IEnumerable<string> PortGuids => portList.Select(p => p.PortGuid);
    }

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

        [SerializeField] PortDataCollection portCollection;
        public IEnumerable<PortData> Ports => portCollection;
        public IEnumerable<string> GetPortGuids() => portCollection.Select(p => p.PortGuid);
        public IEnumerable<string> GetPortGuids(Direction direction) => Ports.Where(p => p.Direction == direction).Select(p => p.PortGuid);
        public PortData GetPortData(string fieldName) => portCollection.FirstOrDefault();
        // Port
        /*
        [Obsolete][SerializeField] PortCollection ports;
        public IEnumerable<KeyValuePair<string, PortData>> Ports => ports;
        public IEnumerable<string> GetPortGuids() => Ports.Select(pair => pair.Value.PortGuid);
        public IEnumerable<string> GetPortGuids(Direction direction) => Ports.Where(pair => pair.Value.Direction == direction).Select(pair => pair.Value.PortGuid);
        public PortData GetPortData(string key) => ports.GetValueOrDefault(key);
        */
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
            //ports = new();
            portCollection = new();

            var fields = GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var field in fields)
            {
                if (field.IsDefined(typeof(PortAttribute), inherit: true))
                {
                    PortAttribute portAttr = field.GetCustomAttribute<PortAttribute>();
                    //string bindingPath = PortAttribute.GetBindingPath(field);

                    PortData newPortData = new(portAttr.Direction, field.Name);

                    //ports.Add(bindingPath, newPortData);
                    portCollection.Add(newPortData);
                    Debug.Log($"added new {newPortData.Direction} Port : {field.Name} {newPortData.PortGuid}");
                }
            }
        }

        public virtual object GetValue(string outputPortGuid) => throw new NotImplementedException();
        public T GetInputValue<T>(string portKey, T defaultValue)
        {
            var inputPort = GetPortData(portKey);
            if (inputPort == null)
                throw new KeyNotFoundException();

            return DialogueTree.GetInputValue(inputPort.PortGuid, defaultValue);
        }
        //

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