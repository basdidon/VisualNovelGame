using System;
using UnityEngine;
using UnityEditor.UIElements;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace H8.FlowGraph
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
                throw new NullReferenceException(
                    $"can't find {fieldInfo.Name} make sure field is <color=blue>public</color> or defined <color=cyan>serializeField</color>."
                );

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
