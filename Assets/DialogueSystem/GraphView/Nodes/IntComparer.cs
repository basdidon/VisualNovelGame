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

    [CreateNodeMenu(menuName = "Logic/Comparer (int)")]
    public class IntComparer : BaseNode
    {
        // backing field
        public int lhs;
        public int rhs;

        [Input(nameof(lhs))]
        public int A => GetInputValue(nameof(A), lhs);
        [Input(nameof(rhs))]
        public int B => GetInputValue(nameof(B), rhs);

        [Output]
        public bool Result => compareType switch
        {
            CompareType.Equal => A == B,
            CompareType.GreaterThan => A > B,
            CompareType.GreaterThanOrEqual => A >= B,
            CompareType.LessThan => A < B,
            CompareType.LessThanOrEqual => A <= B,
            _ => throw new InvalidOperationException(),
        };

        [Selector]
        public CompareType compareType;

        /*
        public override object GetValue(string outputPortGuid)
        {
            return Result;
        }*/
    }
}
