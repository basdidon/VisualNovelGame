using System;
using UnityEngine;

namespace BasDidon.Dialogue.VisualGraphView
{
    [SerializeField]
    [AttributeUsage(AttributeTargets.Field)]
    public class NodeFieldAttribute : Attribute 
    {
        public static readonly Type[] supportedTypes = new[] { typeof(string), typeof(int), typeof(UnityEngine.Object) };
    }
}
