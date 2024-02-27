using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BasDidon.Dialogue.VisualGraphView
{
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class PortAttribute : Attribute
    {
        public string BackingFieldName { get; }
        public bool HasBackingFieldName => !string.IsNullOrEmpty(BackingFieldName);

        public PortAttribute(string backingFieldName = null)
        {
            BackingFieldName = backingFieldName;
        }

        public static IEnumerable<PortData> CreatePortsData(BaseNode baseNode)
        {
            var properties = baseNode.GetType().GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                // Get PortAttribute
                InputAttribute inputAttr = property.GetCustomAttribute<InputAttribute>();
                OutputAttribute outputAttr = property.GetCustomAttribute<OutputAttribute>();

                if (inputAttr != null && outputAttr != null)
                    throw new Exception();

                if (inputAttr == null && outputAttr == null)
                    continue;

                Direction direction = inputAttr != null ? Direction.Input : Direction.Output;

                // use Direction from PortAtrribute to create PortData
                PortData newPortData = new(direction, property.Name);

                Debug.Log($"added new {newPortData.Direction} Port : {property.Name} {newPortData.PortGuid}");
                yield return newPortData;
            }
        }
    }

    public class InputAttribute : PortAttribute
    {
        public InputAttribute(string backingFieldName = null) : base(backingFieldName) { }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class OutputAttribute : PortAttribute
    {
        public OutputAttribute(string backingFieldName = null) : base(backingFieldName) { }
    }
}
