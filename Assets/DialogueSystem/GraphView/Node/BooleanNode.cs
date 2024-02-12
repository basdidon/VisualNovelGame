using UnityEngine;
using UnityEditor.Experimental.GraphView;

namespace BasDidon.Dialogue.VisualGraphView
{
    public class BooleanNode : BaseNode
    {
        [Output] public bool Value { get; private set; }

        public override object ReadValueFromPort(string outputPortGuid)
        {
            return Value;
        }
    }
}