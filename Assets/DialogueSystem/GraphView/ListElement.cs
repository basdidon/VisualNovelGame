using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace BasDidon.Dialogue
{
    using System;
    using System.Reflection;
    using VisualGraphView;

    [System.Serializable]
    public abstract class ListElement
    {
        [SerializeField] protected PortDataCollection ports;
        public IEnumerable<PortData> Ports => ports;
        public IEnumerable<string> GetPortGuids() => ports.Select(p => p.PortGuid);
        public IEnumerable<string> GetPortGuids(Direction direction) => Ports.Where(p => p.Direction == direction).Select(p => p.PortGuid);
        public PortData GetPortDataByGuid(string guid) => ports.FirstOrDefault(p => p.PortGuid == guid);
        public PortData GetPortData(string fieldName) => ports.FirstOrDefault(p => p.FieldName == fieldName);

        public BaseNode BaseNode { get; }
        public DialogueTree DialogueTree => BaseNode.DialogueTree;

        public ListElement()
        {
            Debug.Log("ListElement Init");
            InstantiatePorts();
        }

        public void InstantiatePorts()
        {
            Debug.Log($"<color=yellow>{GetType().Name}</color> InstantiatePorts()");
            ports = new();

            foreach (var portData in PortAttribute.CreatePortsData(this))
            {
                Debug.Log($"<color=yellow>{portData.FieldName}</color> added.");
                ports.Add(portData);
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

        public T GetInputValue<T>(string portKey, T defaultValue)
        {
            var inputPort = GetPortData(portKey);
            if (inputPort == null)
                throw new KeyNotFoundException($"{portKey}");

            return DialogueTree.GetInputValue(inputPort.PortGuid, defaultValue);
        }

    }

    [System.Serializable]
    public class Item : ListElement
    {
        [Input("isEnable")]
        public bool IsEnable { get; }
        public bool isEnable = true;

        [Output]
        public ExecutionFlow Output { get; }

        public string name = string.Empty;
    }
}
