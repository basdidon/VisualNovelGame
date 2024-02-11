using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

namespace BasDidon.Dialogue.VisualGraphView
{
    [Serializable]
    public class PortData
    {
        [field: SerializeField] public string PortGuid { get; private set; }
        [field: SerializeField] public Direction Direction { get; private set; }

        public PortData(Direction direction)
        {
            Direction = direction;

            PortGuid = Guid.NewGuid().ToString();

            Debug.Log($"create port {PortGuid}");
        }
    }
}