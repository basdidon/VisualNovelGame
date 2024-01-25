using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Test
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class DlValueAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class DlClassAttribute : Attribute { }

    public class Test : MonoBehaviour
    {
        //Func<PropertyInfo, bool> IsImplementMyAttr => (prop) => prop.CanRead && Attribute.IsDefined(prop, typeof(MyAttribute));
        [field: SerializeField] public GameObject GameObject { get; set; }
        [field: SerializeField] public Type Type { get; set; }
        [field: SerializeField] public PropertyInfo PropertyInfo { get; set; }

        public UnityEvent<string> UnityEvent;

        private void Start()
        {
            Debug.Log("start");

            UnityEvent?.Invoke("aa");
        }

        public void Yo()
        {
            Debug.Log("Ty");
        }
    }
    
    [CustomEditor(typeof(Test))]
    public class TestEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();

            if (target is not Test test)
                return container;

            List<Type> typeList = new();
            var dropdown = new DropdownField(new(), 0);

            var objField = new PropertyField(serializedObject.FindProperty("<GameObject>k__BackingField"));
            objField.RegisterValueChangeCallback(e=>
            {
                Debug.Log("value Changed");
                if(e.changedProperty.objectReferenceValue != null)
                {
                    typeList = FindClassesInObject(e.changedProperty.objectReferenceValue as GameObject).ToList();
                    dropdown.choices = typeList.Select(prop=>prop.Name).ToList();
                }
                else
                {
                    typeList.Clear();
                    dropdown.choices = new();
                }
                dropdown.value = typeList.FirstOrDefault()?.Name;
            });

            dropdown.RegisterValueChangedCallback(e =>
            {
                Debug.Log($"down {serializedObject.FindProperty("<Type>k__BackingField")}");
            });

            container.Add(objField);
            container.Add(dropdown);

            return container;
        }


        static IEnumerable<string> GetChildInterface(Type parentType)
        {
            // Get all types in the current assembly
            var types = Assembly.GetExecutingAssembly().GetTypes();

            // Filter the types that implement IChildInterface
            return types
                .Where(type => parentType.IsAssignableFrom(type) && type.IsInterface && parentType != type)
                .Select(type => type.Name);
        }

        public IEnumerable<PropertyInfo> FindPropertiesByAttribute(Type attrType)
        {
            // Get all types in the assembly
            var assembly = Assembly.GetExecutingAssembly();
            var allTypes = assembly.GetTypes();

            var propertyInfos = allTypes
                .SelectMany(type =>
                    type.GetProperties()
                    .Where(prop => prop.CanRead && Attribute.IsDefined(prop, attrType))
                );

            return propertyInfos;
        }

        public IEnumerable<Type> FindClassesByAttribute(Type attrType)
        {
            // Get all types in the assembly
            var assembly = Assembly.GetExecutingAssembly();
            var allTypes = assembly.GetTypes();

            var typeInfos = allTypes.Where(type => Attribute.IsDefined(type, attrType));

            return typeInfos;
        }
        
        public IEnumerable<Type> FindClassesInObject(GameObject gameObject)
        {
            return gameObject?
                .GetComponents(typeof(Component))
                .Where(type => Attribute.IsDefined(type.GetType(), typeof(DlClassAttribute)))
                .Select(type => type.GetType());
        }
    }
}