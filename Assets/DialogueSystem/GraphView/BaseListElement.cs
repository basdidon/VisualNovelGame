using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace BasDidon.Dialogue
{
    using System;
    using System.Collections;
    using System.Reflection;
    using VisualGraphView;

    public interface IListElements
    {
        IEnumerable<PortData> GetPorts();
    }

    [System.Serializable]
    public class ListElements<T>: IList<T>,IListElements where T : BaseListElement,new()
    {
        BaseNode BaseNode { get; }

        [SerializeField]
        List<T> listElements;

        public int Count => listElements.Count;
        public bool IsReadOnly => true;

        public T this[int index] { get => listElements[index]; set => listElements[index] = value; }

        public ListElements(BaseNode baseNode)
        {
            listElements = new();

            BaseNode = baseNode;
            BaseNode.OnGetValue += OnGetValue;
        }

        private object OnGetValue(string portGuid)
        {
            BaseListElement listElement = this.FirstOrDefault(e => e.PortsGuid.Contains(portGuid));
            return listElement?.GetValue(portGuid);
        }

        public void Add(T item) => listElements.Add(item);
        public void Clear() => listElements.Clear();
        public bool Contains(T item) => listElements.Contains(item);
        public void CopyTo(T[] array, int arrayIndex) => listElements.CopyTo(array, arrayIndex);
        public bool Remove(T item) => listElements.Remove(item);

        public IEnumerator<T> GetEnumerator() => listElements.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // index related
        public int IndexOf(T item) => listElements.IndexOf(item);
        public void Insert(int index, T item) => listElements.Insert(index, item);
        public void RemoveAt(int index) => listElements.RemoveAt(index);

        // IListElements Implement
        public IEnumerable<PortData> GetPorts() => listElements.SelectMany(e => e.Ports);
    }

    [System.Serializable]
    public abstract class BaseListElement
    {
        [SerializeField] PortDataCollection portCollection;
        public PortData GetPortData(string fieldName) => portCollection.GetPortData(fieldName);
        public IEnumerable<PortData> Ports => portCollection.Ports;
        public IEnumerable<string> PortsGuid => portCollection.PortGuids;

        public BaseNode BaseNode { get; private set; }
        DialogueTree DialogueTree => BaseNode.DialogueTree;

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

        public virtual object GetValue(string portName)
        {
            PropertyInfo propertyInfo = GetType().GetProperty(portName);
            if (propertyInfo == null) throw new Exception();

            return propertyInfo.GetValue(this);
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
