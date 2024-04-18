using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace H8.FlowGraph
{
    using System.Collections;

    [System.Serializable]
    public class ListElements<T> : IList<T>, IListElements where T : BaseListElement, new()
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
        public IEnumerable<PortData> GetPorts()
        {
            return listElements.SelectMany(e => e.Ports);
        }

        public object GetValue(string outputPortGuid)
        {
            var listElement = listElements.FirstOrDefault(listElement => listElement.PortsGuid.Any(portGuid=> portGuid == outputPortGuid));

            if (listElement == null)
                throw new KeyNotFoundException();

            return listElement.GetValue(outputPortGuid);
        }

        public bool TryGetValue(string outputPortGuid, out object value)
        {
            foreach (var listElement in listElements)
            {
                if (listElement.TryGetValue(outputPortGuid, out object _value))
                {
                    value = _value;
                    return true;
                }
            }

            value = default;
            return false;
        }
    }
}
