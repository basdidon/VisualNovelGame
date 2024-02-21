using UnityEngine;

namespace BasDidon.Dialogue.VisualGraphView
{
    [CreateNodeMenu(menuName = "Logic/BooleanNode")]
    public class BooleanNode : BaseNode
    {
        // 
        [field:SerializeField] 
        [Port(PortDirection.Output)]
        public bool Value { get; set; }

        // it work perfectly with field without [SerializeField]
        [Port(PortDirection.Output)]
        public bool field_value;

        // not work
        [Port(PortDirection.Output)]
        public bool AutoPropertyValue { get; set; }

        public override object GetValue(string outputPortGuid)
        {
            return Value;
        }
    }
}