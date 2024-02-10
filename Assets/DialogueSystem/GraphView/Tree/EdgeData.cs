using System;
using System.Linq;
using UnityEngine;

namespace BasDidon.Dialogue.VisualGraphView
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

        public BaseNode GetInputNodeData()
        {
            var result = DialogueTree.Nodes.FirstOrDefault(n => n.InputPortGuids.Contains(InputPortGuid));

            if (result == null)
            {
                throw new Exception($"can't find any node that contain {InputPortGuid}");
            }
            else
            {
                return result;
            }
        }

        public BaseNode GetOutputNodeData() => DialogueTree.Nodes.First(n => n.OutputPortGuids.Contains(OutputPortGuid));
    }
}
