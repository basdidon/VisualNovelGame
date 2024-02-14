using UnityEngine;

namespace BasDidon.Dialogue.VisualGraphView
{
    public class BooleanNode : BaseNode
    {
        [Output]
        //[field: SerializeField]
        public bool Value;

        public override object ReadValueFromPort(string outputPortGuid)
        {
            return Value;
        }
    }
}