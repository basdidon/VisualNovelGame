using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace BasDidon.Dialogue
{
    using VisualGraphView;

    [System.Serializable]
    public abstract class ListElement
    {
        [SerializeField, HideInInspector] PortDataCollection ports;
        public IEnumerable<PortData> Ports => ports;
        public IEnumerable<string> GetPortGuids() => ports.Select(p => p.PortGuid);
        public IEnumerable<string> GetPortGuids(Direction direction) => Ports.Where(p => p.Direction == direction).Select(p => p.PortGuid);
        public PortData GetPortDataByGuid(string guid) => ports.FirstOrDefault(p => p.PortGuid == guid);
        public PortData GetPortData(string fieldName) => ports.FirstOrDefault(p => p.FieldName == fieldName);

        public ListElement()
        {
            InstantiatePorts();
        }

        public void InstantiatePorts()
        {
            Debug.Log($"<color=yellow>{GetType().Name}</color> InstantiatePorts()");
            ports = new();

            foreach (var portData in PortAttribute.CreatePortsData(this))
            {
                ports.Add(portData);
            }
        }

    }

    [System.Serializable]
    public class Item : ListElement
    {
        [Input("isEnable")]
        public bool IsEnable { get; }
        public bool isEnable;

        [Output]
        public ExecutionFlow Output { get; }

        public string name;
    }
}
