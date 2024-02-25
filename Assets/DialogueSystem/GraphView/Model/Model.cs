using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BasDidon.Dialogue.VisualGraphView
{
    [CreateNodeMenu(menuName = "Player")]
    public class Player:BaseNode
    {
        [Input(nameof(m_name))]
        public string Name { get; }
        public string m_name;

        [Output(nameof(money))]
        public int Money { get; }
        public int money;
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class InputAttribute: Attribute
    {
        public string FieldName { get; }
        public bool HasFieldName => !string.IsNullOrEmpty(FieldName);

        public InputAttribute(string fieldName = null)
        {
            FieldName = fieldName;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class OutputAttribute : Attribute
    {
        public string FieldName { get; }
        public bool HasFieldName => !string.IsNullOrEmpty(FieldName);

        public OutputAttribute(string fieldName = null)
        {
            FieldName = fieldName;
        }
    }
}
