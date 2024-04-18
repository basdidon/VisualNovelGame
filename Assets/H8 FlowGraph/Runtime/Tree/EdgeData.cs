using System;
using System.Linq;
using UnityEngine;

namespace H8.FlowGraph
{
    [Serializable]
    public class EdgeData
    {
        [field: SerializeField] public string EdgeGuid { get; private set; }
        [field: SerializeField] public string OutputPortGuid { get; private set; }
        [field: SerializeField] public string InputPortGuid { get; private set; }

        public EdgeData(string outputPortGuid, string inputPortGuid)
        {
            EdgeGuid = Guid.NewGuid().ToString();   // generate guid

            OutputPortGuid = outputPortGuid;
            InputPortGuid = inputPortGuid;
        }

        public static EdgeData GetEdgeData(GraphTree dialogueTree, string edgeGuid) => dialogueTree.Edges.First(e => e.EdgeGuid == edgeGuid);
    }
}
