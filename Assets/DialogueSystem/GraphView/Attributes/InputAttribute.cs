using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


namespace H8.GraphView
{
    public class InputAttribute : PortAttribute
    {
        public InputAttribute(string backingFieldName = null) : base(backingFieldName) { }

        public override Direction Direction => Direction.Input;
    }
}
