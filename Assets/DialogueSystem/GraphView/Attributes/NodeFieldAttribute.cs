using System;
using UnityEngine;
using UnityEditor.UIElements;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace BasDidon.Dialogue.VisualGraphView
{
    [SerializeField]
    [AttributeUsage(AttributeTargets.Field)]
    public class NodeFieldAttribute : Attribute 
    {
        public static readonly Type[] supportedTypes = new[] { typeof(string), typeof(int), typeof(UnityEngine.Object) };

        public PropertyField CreatePropertyField(FieldInfo fieldInfo,SerializedObject serializeObject)
        {
            var serializeProperty = serializeObject.FindProperty(fieldInfo.Name);

            if (serializeProperty == null)
                throw new NullReferenceException($"can't find {fieldInfo.Name}");

            bool isSupportedType = supportedTypes.Contains(fieldInfo.FieldType);
            bool isSubClassOfSupportType = supportedTypes.Any(t => fieldInfo.FieldType.IsSubclassOf(t));

            if ( isSupportedType || isSubClassOfSupportType )
            {
                return new PropertyField(serializeProperty);
            }

            throw new System.InvalidOperationException();
        }

        public PropertyField CreateUnbindPropertyField(FieldInfo fieldInfo)
        {
            return new PropertyField() { name = fieldInfo.Name };
        }
    }
}
