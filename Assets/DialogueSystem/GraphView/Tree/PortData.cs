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
    public class PortDataCollection : ICollection<PortData>
    {
        [SerializeField] List<PortData> portList = new();

        public void Add(PortData item)
        {
            if (!portList.Any(p => p.PortGuid == item.PortGuid || p.FieldName == item.FieldName))
            {
                portList.Add(item);
            }
        }

        public void Clear() => portList.Clear();
        public bool Contains(PortData item) => portList.Contains(item);
        public void CopyTo(PortData[] array, int arrayIndex) => portList.CopyTo(array, arrayIndex);
        public bool Remove(PortData item) => portList.Remove(item);
        public IEnumerator<PortData> GetEnumerator() => portList.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int IndexOf(PortData item)
        {
            return portList.IndexOf(item);
        }

        public void Insert(int index, PortData item)
        {
            portList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            portList.RemoveAt(index);
        }

        public IEnumerable<PortData> Ports => portList.AsEnumerable();
        public IEnumerable<string> PortGuids => portList.Select(p => p.PortGuid);

        public int Count => portList.Count;
        public bool IsReadOnly => true;

        public PortData this[int index] { get => portList[index]; set => portList[index] = value; }
    }

}