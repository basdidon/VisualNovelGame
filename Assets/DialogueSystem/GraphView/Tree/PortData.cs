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
        [field: SerializeField] public Type Type { get; private set; }

        public PortData(Direction direction, Type type)
        {
            Direction = direction;

            PortGuid = Guid.NewGuid().ToString();
            Type = type;
            //Debug.Log($"create port {PortGuid}");
        }
    }
}