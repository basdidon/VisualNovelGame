using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BasDidon.Dialogue.VisualGraphView
{
    [CreateNodeMenu(menuName = "Player")]
    public class Player:BaseNode
    {
        [Output]
        public string Name => "Bas";

        [Output]
        public int Money => 1000;
    }

    [AttributeUsage(AttributeTargets.Property)]
    public abstract class AwesomePortAttribute : Attribute
    {
        public string FieldName { get; }
        public bool HasFieldName => !string.IsNullOrEmpty(FieldName);

        public AwesomePortAttribute(string fieldName = null)
        {
            FieldName = fieldName;
        }
    }

    public class InputAttribute: AwesomePortAttribute
    {
        public InputAttribute(string fieldName = null): base(fieldName){}

        public static IEnumerable<PortData> CreatePortsData(BaseNode baseNode)
        {
            var properties = baseNode.GetType().GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance);
            Debug.Log(properties.Length);
            foreach (var property in properties)
            {
                // Get PortAttribute
                InputAttribute portAttr = property.GetCustomAttribute<InputAttribute>();

                if (portAttr != null)
                {
                    // use Direction from PortAtrribute to create PortData
                    PortData newPortData = new(Direction.Input, property.Name);

                    Debug.Log($"added new {newPortData.Direction} Port : {property.Name} {newPortData.PortGuid}");
                    yield return newPortData;
                }
            }
        }

        public static void CreatePorts(NodeView nodeView, BaseNode baseNode)
        {
            // create ports
            foreach (var port in baseNode.Ports)
            {
                PropertyInfo property = baseNode.GetType().GetProperty(port.FieldName);
                Type type = property.PropertyType;

                NodeElementFactory.DrawPort(type, port, nodeView, port.FieldName);
            }
        }

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class OutputAttribute : AwesomePortAttribute
    {
        public OutputAttribute(string fieldName = null):base(fieldName) {}

        public static IEnumerable<PortData> CreatePortsData(BaseNode baseNode)
        {
            var properties = baseNode.GetType().GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance);
            Debug.Log(properties.Length);
            foreach (var property in properties)
            {
                // Get PortAttribute
                OutputAttribute portAttr = property.GetCustomAttribute<OutputAttribute>();

                if (portAttr != null)
                {
                    // use Direction from PortAtrribute to create PortData
                    PortData newPortData = new(Direction.Output, property.Name);

                    Debug.Log($"added new {newPortData.Direction} Port : {property.Name} {newPortData.PortGuid}");
                    yield return newPortData;
                }
            }
        }

        public static void CreatePorts(NodeView nodeView, BaseNode baseNode)
        {
            // create ports
            foreach (var port in baseNode.Ports)
            {
                PropertyInfo property = baseNode.GetType().GetProperty(port.FieldName);
                Type type = property.PropertyType;

                NodeElementFactory.DrawPort(type, port, nodeView, port.FieldName);
            }
        }

    }
}
