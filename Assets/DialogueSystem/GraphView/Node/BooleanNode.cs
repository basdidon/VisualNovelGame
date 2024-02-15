using UnityEngine;

namespace BasDidon.Dialogue.VisualGraphView
{
    [CreateNodeMenu(menuName = "Logic/BooleanNode")]
    public class BooleanNode : BaseNode
    {
        [Output]
        public bool Value;

        public override object ReadValueFromPort(string outputPortGuid)
        {
            return Value;
        }
    }
}