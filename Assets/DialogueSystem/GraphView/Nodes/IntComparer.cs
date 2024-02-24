using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BasDidon.Dialogue.VisualGraphView
{
    public enum CompareType
    {
        Equal,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual
    }

    [CreateNodeMenu(menuName = "Logic/Comparer(int)")]
    public class IntComparer : BaseNode
    {
        [Port(PortDirection.Input)]
        public int a;
        [Port(PortDirection.Input)]
        public int b;

        [Port(PortDirection.Output, PortFieldStyle.Hide)]
        public bool result;

        [Selector]
        public CompareType compareType;

        public override object GetValue(string outputPortGuid)
        {
            int A = GetInputValue("a", a);
            int B = GetInputValue("b", b);

            return compareType switch
            {
                CompareType.Equal => A == B,
                CompareType.GreaterThan => A > B,
                CompareType.GreaterThanOrEqual => A >= B,
                CompareType.LessThan => A < B,
                CompareType.LessThanOrEqual => A <= B,
                _ => throw new InvalidOperationException(),
            };
        }
    }
}
