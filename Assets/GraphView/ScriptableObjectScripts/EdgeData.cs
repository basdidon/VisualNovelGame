using System;
using System.Linq;
using UnityEngine;

namespace Graphview.NodeData
{
    [Serializable]
    public class EdgeData
    {
        [field: SerializeField] public DialogueTree DialogueTree { get; private set; }
        [field: SerializeField] public string EdgeGuid { get; private set; }
        [field: SerializeField] public string OutputPortGuid { get; private set; }
        [field: SerializeField] public string InputPortGuid { get; private set; }

        public EdgeData(DialogueTree dialogueTree, string outputPortGuid, string inputPortGuid)
        {
            DialogueTree = dialogueTree;
            EdgeGuid = Guid.NewGuid().ToString();   // generate guid
            OutputPortGuid = outputPortGuid;
            InputPortGuid = inputPortGuid;
        }

        public static EdgeData GetEdgeData(DialogueTree dialogueTree, string edgeGuid) => dialogueTree.Edges.First(e => e.EdgeGuid == edgeGuid);

        public NodeData GetInputNodeData()
        {
            var result = DialogueTree.Nodes.FirstOrDefault(n => n.InputPortGuids.Contains(InputPortGuid));

            foreach(var node in DialogueTree.Nodes)
            {
                Debug.Log($"{node.name} {node.InputPortGuids.Count()}");
                foreach(var inputPortGuid in node.InputPortGuids)
                {
                    Debug.Log($"{InputPortGuid} <> {inputPortGuid} -> {InputPortGuid == inputPortGuid}");
                    
                }
            }

            if (result == null)
            {
                throw new Exception($"can't find any node that contain {InputPortGuid}");
            }
            else
            {
                return result;
            }
        }

        public NodeData GetOutputNodeData() => DialogueTree.Nodes.First(n => n.OutputPortGuids.Contains(OutputPortGuid));
    }
}
