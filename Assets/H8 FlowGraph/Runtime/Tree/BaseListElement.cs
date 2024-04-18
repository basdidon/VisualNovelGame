using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace H8.FlowGraph
{
    [System.Serializable]
    public abstract class BaseListElement
    {
        [SerializeField] PortDataCollection portCollection;
        public PortData GetPortData(string fieldName) => portCollection.GetPortData(fieldName);
        public IEnumerable<PortData> Ports => portCollection.Ports;
        public IEnumerable<string> PortsGuid => portCollection.PortGuids;

        [field:SerializeField] public BaseNode BaseNode { get; private set; }
        GraphTree DialogueTree => BaseNode.GraphTree;

        public void Initialize(BaseNode baseNode)
        {
            InstantiatePorts();
            BaseNode = baseNode;
        }

        public void InstantiatePorts()
        {
            Debug.Log($"<color=yellow>{GetType().Name}</color> InstantiatePorts()");
            portCollection = new();

            foreach (var portData in PortAttribute.CreatePortsData(this))
            {
                Debug.Log($"<color=yellow>{portData.FieldName}</color> added.");
                portCollection.Add(portData);
            }
        }

        public virtual object GetValue(string outputPortGuid)
        {
            PropertyInfo propertyInfo = GetType().GetProperty(Ports.FirstOrDefault(p=>p.PortGuid == outputPortGuid).FieldName);
            if (propertyInfo == null) throw new Exception();

            return propertyInfo.GetValue(this);
        }

        public virtual bool TryGetValue(string outputPortGuid, out object value)
        {
            value = default;
            
            var port = Ports.FirstOrDefault(p => p.PortGuid == outputPortGuid);
            if (port == null) return false;

            PropertyInfo propertyInfo = GetType().GetProperty(port.FieldName);
            if (propertyInfo == null) return false;

            value = propertyInfo.GetValue(this);
            return true;
        }

        protected T GetInputValue<T>(string portKey, T defaultValue)
        {
            var inputPort = portCollection.GetPortData(portKey);
            if (inputPort == null)
                throw new KeyNotFoundException($"{portKey}");

            return DialogueTree.GetInputValue(inputPort.PortGuid, defaultValue);
        }

    }
}
