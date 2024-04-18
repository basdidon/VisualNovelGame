using System.Collections.Generic;

namespace H8.FlowGraph
{
    public interface IListElements
    {
        IEnumerable<PortData> GetPorts();
        object GetValue(string outputPortGuid);
        bool TryGetValue(string outputPortGuid, out object value);
    }
}
