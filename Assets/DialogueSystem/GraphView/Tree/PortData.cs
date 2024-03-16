using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace BasDidon.Dialogue.VisualGraphView
{
    [Serializable]
    public class PortData
    {
        [field: SerializeField] public string FieldName { get; private set; }

        [field: SerializeField] public string PortGuid { get; private set; }
        [field: SerializeField] public Direction Direction { get; private set; }

        public PortData(Direction direction, string fieldName)
        {
            FieldName = fieldName;

            PortGuid = Guid.NewGuid().ToString();
            Direction = direction;
        }
    }

    [Serializable]
    public class PortDataCollection //: ICollection<PortData>
    {
        [SerializeField]List<PortData> portList = new();

        public void Add(PortData item)
        {
            if (!portList.Any(p => p.PortGuid == item.PortGuid || p.FieldName == item.FieldName))
            {
                portList.Add(item);
            }
        }


        public void Clear() => portList.Clear();

        public IEnumerable<PortData> Ports => portList.AsEnumerable();
        public IEnumerable<string> PortGuids => portList.Select(p => p.PortGuid);
        public IEnumerable<string> GetPortGuids() => portList.Select(p => p.PortGuid);
        public IEnumerable<string> GetPortGuids(Direction direction) => Ports.Where(p => p.Direction == direction).Select(p => p.PortGuid);
        public PortData GetPortDataByGuid(string guid) => portList.FirstOrDefault(p => p.PortGuid == guid);
        public PortData GetPortData(string fieldName) => portList.FirstOrDefault(p => p.FieldName == fieldName);

        public int Count => portList.Count;
        public bool IsReadOnly => true;

    }
    /*
    [System.Serializable]
    public class NodePortDataCollection :PortDataCollection
    {
        List<ListElementPortDataCollection> listElementPortCollection = new();

        public void AddListElement(ListElementPortDataCollection collection)
        {
            listElementPortCollection.Add(collection);
        }

        public IEnumerable<string> GetPortsGuid()
        {
            var listElementportsGuid = listElementPortCollection.SelectMany(c => c.PortGuids);

            return PortGuids.Union(listElementportsGuid);
        }
    }

    [System.Serializable]
    public class ListElementPortDataCollection : PortDataCollection
    {

    }*/
}