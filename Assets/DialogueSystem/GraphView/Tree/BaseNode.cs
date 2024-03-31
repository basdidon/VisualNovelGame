using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace H8.GraphView
{
    public abstract class BaseNode : ScriptableObject
    {
        [SerializeField] GraphTree dialogueTree;
        public GraphTree GraphTree
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

        public IEnumerable<IListElements> GetAllListElement()
        {
            var fieldsInfo = GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var fieldInfo in fieldsInfo)
            {
                Type fieldType = fieldInfo.FieldType;
                if( typeof(IListElements).IsAssignableFrom(fieldType) && fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(ListElements<>))
                {
                    yield return fieldInfo.GetValue(this) as IListElements;
                }
            }
        }

        public IEnumerable<PortData> Ports
        {
            get
            {
                var allListElementPort = GetAllListElement()?.SelectMany(e => e.GetPorts());

                if (allListElementPort != null)
                {
                    return portCollection.Ports.Union(allListElementPort);
                }
                return portCollection.Ports;

            }
        }
        public IEnumerable<string> GetPortGuids() => Ports.Select(p => p.PortGuid);
        public IEnumerable<string> GetPortGuids(Direction direction) => Ports.Where(p=>p.Direction == direction).Select(p => p.PortGuid);
        public PortData GetPortDataByGuid(string guid) => Ports.FirstOrDefault(p => p.PortGuid == guid);
        public PortData GetPortData(string fieldName) => Ports.FirstOrDefault(p=>p.FieldName == fieldName);
        
        public virtual void Initialize(Vector2 position, GraphTree dialogueTree)
        {
            Debug.Log($"{GetType()} initialize");
            Id = $"{Guid.NewGuid()}";
            GraphPosition = position;
            GraphTree = dialogueTree;
            name = GetType().Name;
            title = GetType().Name;

            InstantiatePorts();

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
            Debug.Log($"<color=yellow>{GetType().Name}</color> InstantiatePorts()");
            portCollection = new();

            foreach(var portData in PortAttribute.CreatePortsData(this))
            {
                portCollection.Add(portData);
            }
        }

        public virtual object GetValue(string outputPortGuid) 
        {
            if(portCollection.Ports.Any(p=>p.PortGuid == outputPortGuid))
            {
                var port = GetPortDataByGuid(outputPortGuid);
                if (port == null) throw new Exception($"cannot find port : {outputPortGuid}");

                PropertyInfo propertyInfo = GetType().GetProperty(port.FieldName);
                if (propertyInfo == null)
                    throw new Exception($"field name {port.FieldName} is not found.");
               
                return propertyInfo.GetValue(this);
            }

            foreach(var listElements in GetAllListElement())
            {
                if(listElements.TryGetValue(outputPortGuid,out object value))
                {
                    return value;
                }
            }

            throw new InvalidOperationException();
        }

        protected  T GetInputValue<T>(string portKey, T defaultValue)
        {
            var inputPort = GetPortData(portKey);
            if (inputPort == null)
                throw new KeyNotFoundException($"{portKey}");

            return GraphTree.GetInputValue(inputPort.PortGuid, defaultValue);
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