using UnityEngine;
using System;

namespace BasDidon.Dialogue.VisualGraphView
{
    [CreateNodeMenu(menuName = "Logic/BooleanNode")]
    public class BooleanNode : BaseNode
    {
        [Port(PortDirection.Output)]
        public bool value;

        public override object GetValue(string outputPortGuid)
        {
            return value;
        }
    }
}