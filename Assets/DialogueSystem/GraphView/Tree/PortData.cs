using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

namespace BasDidon.Dialogue.VisualGraphView
{
    [Serializable]
    public class PortData
    {
        [field: SerializeField] public string FieldName { get; private set; }

        [field: SerializeField] public string PortGuid { get; private set; }
        [field: SerializeField] public Direction Direction { get; private set; }

        public PortData(Direction direction, string fieldName)
        {
            FieldName = fieldName;

            PortGuid = Guid.NewGuid().ToString();
            Direction = direction;
        }
    }
}