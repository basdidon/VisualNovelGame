using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using System.Linq;
using System.Reflection;
using System.Collections;

namespace BasDidon.Dialogue.VisualGraphView
{
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
        [SerializeField, HideInInspector] 
        string title = string.Empty;
        [field:SerializeField] public string NodeName { get; set; }

        [field: SerializeField] public Vector2 GraphPosition { get; set; }             // position on graphview

        [SerializeField ,HideInInspector ] PortDataCollection portCollection;

        // set to private later
        [SerializeReference]
        protected List<IListElements> listElementCollection;

        public IEnumerable<PortData> Ports
        {
            get
            {
                Debug.Log(GetType());
                if(listElementCollection == null)
                    Debug.Log("GG");
                
                var LEC = listElementCollection?.Where(e=>e.GetPorts().Count() > 0).SelectMany(e => e.GetPorts());
                
                return       portCollection.Ports.Union(LEC);

            }
        }
        public IEnumerable<string> GetPortGuids() => Ports.Select(p => p.PortGuid);
        public IEnumerable<string> GetPortGuids(Direction direction) => Ports.Where(p=>p.Direction == direction).Select(p => p.PortGuid);
        public PortData GetPortDataByGuid(string guid) => Ports.FirstOrDefault(p => p.PortGuid == guid);
        public PortData GetPortData(string fieldName) => Ports.FirstOrDefault(p=>p.FieldName == fieldName);

        public event Func<string,object> OnGetValue;
        
        public virtual void Initialize(Vector2 position, DialogueTree dialogueTree)
        {
            Debug.Log($"{GetType()} initialize");
            Id = $"{Guid.NewGuid()}";
            GraphPosition = position;
            DialogueTree = dialogueTree;
            name = GetType().Name;
            title = GetType().Name;

            InstantiatePorts();

            listElementCollection = new();
            Debug.Log($"c {listElementCollection.Count}");

            dialogueTree.Nodes.Add(this);
            AssetDatabase.AddObjectToAsset(this, dialogueTree);
            SaveChanges();
        }

        private void OnValidate()
        {
            title = string.IsNullOrEmpty(NodeName) || string.IsNullOrWhiteSpace(NodeName) ? GetType().Name : NodeName;
        }

        public void InstantiatePorts()
        {
            Debug.Log("d");
            Debug.Log($"<color=yellow>{GetType().Name}</color> InstantiatePorts()");
            portCollection = new();

            foreach(var portData in PortAttribute.CreatePortsData(this))
            {
                portCollection.Add(portData);
            }
        }

        public virtual object GetValue(string outputPortGuid) 
        {
            var port = GetPortDataByGuid(outputPortGuid);
            if (port == null) throw new Exception();

            PropertyInfo propertyInfo = GetType().GetProperty(port.FieldName);
            if (propertyInfo == null) throw new Exception();

            return propertyInfo.GetValue(this);
        }

        protected  T GetInputValue<T>(string portKey, T defaultValue)
        {
            var inputPort = GetPortData(portKey);
            if (inputPort == null)
                throw new KeyNotFoundException($"{portKey}");

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
}