using System.Collections.Generic;

namespace BasDidon.Dialogue
{
    using VisualGraphView;

    public interface IListElements
    {
        IEnumerable<PortData> GetPorts();
        object GetValue(string outputPortGuid);
        bool TryGetValue(string outputPortGuid, out object value);
    }
}
