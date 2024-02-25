using System;
using System.Reflection;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;
using UnityEditor;

namespace BasDidon.Dialogue.VisualGraphView
{
    public enum PortDirection
    {
        Input,
        Output
    }

    public enum PortFieldStyle
    {
        Show,
        Disable,
        Hide
    }

    [SerializeField]
    [AttributeUsage(AttributeTargets.Field)]
    public class PortAttribute : Attribute
    {
        public Direction Direction { get; }

        public PortFieldStyle PortFieldStyle { get; }

        public PortAttribute(PortDirection direction, PortFieldStyle portFieldStyle = PortFieldStyle.Show)
        {
            Direction = direction switch
            {
                PortDirection.Input => Direction.Input,
                PortDirection.Output => Direction.Output,
                _ => throw new InvalidOperationException()
            };

            PortFieldStyle = portFieldStyle;
        }

        public static Type GetTypeOfMember(MemberInfo member)
        {
            if (!member.IsDefined(typeof(PortAttribute), inherit: true))
                throw new Exception("The member is not decorated with the PortAttribute.");

            Debug.Log($"{member.DeclaringType.Name} {member.Name} ({member.MemberType})");

            if (member.MemberType == MemberTypes.Property)
            {
                return (member as PropertyInfo).PropertyType;
            }
            else if (member.MemberType == MemberTypes.Field)
            {
                if (member.Name.Contains(">k__BackingField"))
                {
                    var propertyName = member.Name.Replace("<", "").Replace(">k__BackingField", "");
                    var property = member.DeclaringType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    return property.PropertyType;
                }
                else
                {
                    return (member as FieldInfo).FieldType;
                }
            }
            else
            {
                throw new InvalidOperationException("Unsupported member type.");
            }
        }

        public static string GetBindingPath(MemberInfo member)
        {
            if (!member.IsDefined(typeof(PortAttribute), inherit: true))
                throw new Exception("The member is not decorated with the PortAttribute.");

            return member.MemberType switch
            {
                MemberTypes.Property => $"<{member.Name}>k__BackingField",
                MemberTypes.Field => member.Name,
                _ => throw new InvalidOperationException("Unsupported member type.")
            };
        }


    }

    public static class PortAttributeExtensions
    {
        #region Port
        public static IEnumerable<PortData> CreatePortsData(this BaseNode baseNode)
        { 
            var fields = baseNode.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                // Get PortAttribute
                PortAttribute portAttr = field.GetCustomAttribute<PortAttribute>();

                if (portAttr != null)
                {
                    // use Direction from PortAtrribute to create PortData
                    PortData newPortData = new(portAttr.Direction, field.Name);

                    Debug.Log($"added new {newPortData.Direction} Port : {field.Name} {newPortData.PortGuid}");
                    yield return newPortData;
                }
            }
        }

        public static void CreatePortsView(this NodeView nodeView, BaseNode baseNode)
        {
            // create ports
            foreach (var port in baseNode.Ports)
            {
                var serializeProperty = nodeView.SerializedObject.FindProperty(port.FieldName);
                FieldInfo fieldInfo = baseNode.GetType().GetField(port.FieldName);
                Type type = fieldInfo.FieldType;
                string propertyName = StringHelper.GetFieldName(port.FieldName);

                var portAttr = fieldInfo.GetCustomAttribute<PortAttribute>(true);
                var portFieldStyle = portAttr.PortFieldStyle;

                NodeElementFactory.DrawPortWithField(serializeProperty, type, port, nodeView, propertyName, portFieldStyle);
            }
        }

        #endregion


        public static IEnumerable<PortData> CreateInputPortsData(this BaseNode baseNode)
        {
            var properties = baseNode.GetType().GetProperties(BindingFlags.GetProperty | BindingFlags.Public);
            foreach(var property in properties)
            {
                // Get PortAttribute
                PortAttribute portAttr = property.GetCustomAttribute<PortAttribute>();

                if(portAttr != null)
                {
                    // use Direction from PortAtrribute to create PortData
                    PortData newPortData = new(Direction.Input, property.Name);

                    Debug.Log($"added new {newPortData.Direction} Port : {property.Name} {newPortData.PortGuid}");
                    yield return newPortData;
                }
            }
        }
    }
}