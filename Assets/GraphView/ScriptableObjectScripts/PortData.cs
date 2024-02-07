using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.Experimental.GraphView;
using System.Linq;

namespace Graphview.NodeData
{
    [Serializable]
    public class PortData
    {
        [field: SerializeField] DialogueTree DialogueTree { get; set; }

        [field: SerializeField] public string PortGuid { get; private set; }
        [field: SerializeField] public Direction Direction { get; private set; }

        public PortData(DialogueTree dialogueTree, Direction direction)
        {
            DialogueTree = dialogueTree;
            Direction = direction;

            PortGuid = Guid.NewGuid().ToString();

            Debug.Log($"create port {PortGuid}");
        }

        public IEnumerable<EdgeData> EdgesData => DialogueTree.Edges.Where(e => e.InputPortGuid == PortGuid || e.OutputPortGuid == PortGuid);

        public IEnumerable<NodeData> GetConnectedNode()
        {
            if (Direction == Direction.Input)
            {
                return EdgesData.Select(e => e.GetOutputNodeData());
            }
            if (Direction == Direction.Output)
            {
                return EdgesData.Select(e => e.GetInputNodeData());
            }

            throw new Exception();
        }

        public List<T> GetConnectedNodeOfType<T>() => GetConnectedNode().OfType<T>().ToList();

        public IEnumerable<string> GetOthersPortGuid()
        {
            if (Direction == Direction.Input)
            {
                return EdgesData.Select(e => e.OutputPortGuid);
            }
            if (Direction == Direction.Output)
            {
                return EdgesData.Select(e => e.InputPortGuid);
            }

            throw new Exception();
        }
    }
}