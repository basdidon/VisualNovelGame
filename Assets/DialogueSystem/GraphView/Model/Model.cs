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
        [Input]
        public string Name { get; }
        public string m_name;
        

        public int Money { get; }
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
}
