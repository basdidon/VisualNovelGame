using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace BasDidon.Dialogue.VisualGraphView
{
    public class NodeView : Node
    {
        public DialogueGraphView GraphView { get; private set; }
        public SerializedObject SerializedObject { get; private set; }
        BaseNode baseNode;

        public void Initialize(BaseNode nodeData, DialogueGraphView graphView)
        {
            GraphView = graphView;
            SetPosition(new Rect(nodeData.GraphPosition, Vector2.zero));
            viewDataKey = nodeData.Id;
            userData = nodeData;
            name = nodeData.GetType().Name;
            title = name;

            Label titleLabel = (Label) titleContainer.ElementAt(0);
            titleLabel.bindingPath = "title";

            SerializedObject = new(nodeData);
            mainContainer.Bind(SerializedObject);

            baseNode = nodeData;
        }

        public override void OnSelected()
        {
            base.OnSelected();
            Selection.activeObject = baseNode;
        }

        public virtual void OnDrawNodeView(BaseNode baseNode)
        {
            Debug.Log($"StartDrawNode : <color=yellow>{baseNode.GetType().Name}</color>");

            CreatePorts(baseNode);
            CreateSelectors(baseNode);
            CreateNodeFields(baseNode);
            CreateListFields(baseNode);

            var fields = baseNode.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var field in fields)
            {   
                if (typeof(IList).IsAssignableFrom(field.FieldType) && field.FieldType.GetGenericTypeDefinition() == typeof(List<>) && field.FieldType.GetGenericArguments()[0].IsSubclassOf(typeof(ListElement)))
                {
                    var value = field.GetValue(baseNode) as IList;

                    Debug.Log($"---->{field.FieldType} {field.Name}");
                    Debug.Log($"----> {value.Count}");

                    var newList = new NewGraphListView(SerializedObject.FindProperty(field.Name), this, field.FieldType.GetGenericArguments()[0]);
                    extensionContainer.Add(newList);
                    RefreshExpandedState();
                    //BaseGraphList baseGraphList = new();
                }
                /*
            if(field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type[] genericArguments = field.FieldType.GetGenericArguments();
                Type listElementType = typeof(ListElement<>).MakeGenericType(genericArguments);

                object fieldValue = field.GetValue(baseNode) as IList;

                var choicesProp = SerializedObject.FindProperty("new_choices");
                var choicesPropEnumerator = choicesProp.GetEnumerator();

                var counter = 0;
                while (choicesPropEnumerator.MoveNext())
                {
                    var cur = choicesPropEnumerator.Current;

                }

                /*
                fieldValue
                ListElement<> a = Convert.ChangeType(fieldValue, listElementType);
                if(fieldValue is IList<> list)
                {
                    Debug.Log($"Hooray {list.Count}");
                }
                else
                {
                    Debug.Log("-------------");

                }
                var fieldsInGenericType = listElementType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            }
        }

        var typesWithGeneric = baseNode.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(f => f.FieldType.IsGenericType && f.FieldType.GetGenericTypeDefinition() == typeof(ListElement<>));

        Debug.Log($"-> {typesWithGeneric.Count()}");

        RefreshExpandedState();*/
            }
        }

        Type GetFieldOrPropertyType(Type baseClass, string name) => GetFieldOrPropertyType(baseClass, name, out _); 
        Type GetFieldOrPropertyType(Type baseClass,string name, out bool isField)
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            if (baseClass.GetField(name, flags) is FieldInfo portField)
            {
                isField = true;
                return portField.FieldType;
            }
            else if (baseClass.GetProperty(name, flags) is PropertyInfo portProperty)
            {
                isField = false;
                return portProperty.PropertyType;
            }
            else
            {
                throw new Exception($"<color=red>{baseClass.GetType().Name}</color> {name}");
            }
        }

        void CreatePorts(BaseNode baseNode)
        {
            // create ports
            foreach (var port in baseNode.Ports)
            {
                PropertyInfo property = baseNode.GetType().GetProperty(port.FieldName);
                if (property == null)
                    continue;

                Type type = property.PropertyType;

                var portAttr = property.GetCustomAttribute<PortAttribute>();

                if (portAttr == null)
                    continue;

                if (portAttr.HasBackingFieldName)
                {
                    var serializeProperty = SerializedObject.FindProperty(portAttr.BackingFieldName);

                    if (serializeProperty != null)
                        NodeElementFactory.DrawPortWithField(serializeProperty, type, port, this, port.FieldName);
                    else
                        Debug.LogError(port.FieldName);
                }
                else
                {
                    // DrawPortWithNoSideField
                    NodeElementFactory.DrawPort(type, port, this, port.FieldName);
                }

            }
        }

        void CreateSelectors(BaseNode baseNode)
        {
            var selectorMembers = baseNode.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(m => m.IsDefined(typeof(SelectorAttribute), inherit: true));

            foreach (var member in selectorMembers)
            {
                CreateSelector(member);
            }
        }

        void CreateSelector(FieldInfo fieldInfo)
        {
            EnumField enumField = new(StringHelper.ToCapitalCase(fieldInfo.Name)) { bindingPath = fieldInfo.Name };
            extensionContainer.Add(enumField);
        }

        void CreateNodeFields(BaseNode baseNode)
        {
            Type[] types = new[] { typeof(string), typeof(int) ,typeof(UnityEngine.Object)};

            var nodeFields = baseNode.GetType().GetFields(BindingFlags.Instance| BindingFlags.NonPublic| BindingFlags.Public)
                .Where(m => m.IsDefined(typeof(NodeFieldAttribute), inherit: true));

            foreach (var field in nodeFields)
            {
                CreateNodeField(field);
            }
        }

        void CreateNodeField(FieldInfo fieldInfo)
        {
            var serializeProperty = SerializedObject.FindProperty(fieldInfo.Name);

            if (serializeProperty == null)
                throw new NullReferenceException($"can't find {fieldInfo.Name}");

            Type fieldType = GetFieldOrPropertyType(baseNode.GetType(), fieldInfo.Name);

            if (NodeFieldAttribute.supportedTypes.Contains(fieldType) || NodeFieldAttribute.supportedTypes.Any(t=>fieldType.IsSubclassOf(t)))
            {
                var propField = new PropertyField(serializeProperty);
                extensionContainer.Add(propField);
            }
            else
            {
                throw new System.InvalidOperationException();
            }
        }

        void CreateListFields(BaseNode baseNode)
        {
            var ListFields = baseNode.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(f => f.IsDefined(typeof(ListFieldAttribute), inherit: true));


            Debug.Log(ListFields.Count());

            foreach(var listField in ListFields)
            {
                CreateListField(listField);
            }
        }

        void CreateListField(FieldInfo fieldInfo)
        {
            ListFieldAttribute attr = fieldInfo.GetCustomAttribute<ListFieldAttribute>(true);
            var serializeProperty = SerializedObject.FindProperty(fieldInfo.Name);

            if (serializeProperty == null)
                throw new NullReferenceException($"can't find {fieldInfo.Name}");

            var listview = GraphListViewFactory.GetGraphListView(attr.CreatorType,serializeProperty,this);
            extensionContainer.Add(listview);
        }

        public void RemovePort(Port port)
        {
            Debug.Log("removing port");
            if (port.connected)
            {
                Debug.Log("-- deleting edges");
                GraphView.DeleteElements(port.connections);
                port.DisconnectAll();
            }

            GraphView.RemoveElement(port);
        }
    }

    public static class GraphListViewFactory
    {
        public static GraphListView GetGraphListView(Type type,SerializedProperty serializedProperty, NodeView nodeView)
        {
            return Activator.CreateInstance(type,new object[] {serializedProperty,nodeView}) as GraphListView;
        }
    }

    public class BaseGraphList : VisualElement
    {
        public VisualElement PortsContainer { get; }
        public VisualElement InputContainer { get; }
        public VisualElement OutputContainer { get; }
        public VisualElement ContentContainer { get; }

        public BaseGraphList()
        {
            PortsContainer = new();
            PortsContainer.style.flexDirection = FlexDirection.Row;
            Add(PortsContainer);

            InputContainer = new();
            InputContainer.style.flexGrow = 1;
            PortsContainer.Add(InputContainer);

            OutputContainer = new();
            OutputContainer.style.flexGrow = 1;
            PortsContainer.Add(OutputContainer);

            ContentContainer = new();
            Add(ContentContainer);  
        }
    }

    public class NewGraphListView : VisualElement
    {
        public SerializedProperty SerializedProperty { get; }
        public NodeView NodeView { get; }
        public DialogueGraphView GraphView => NodeView.GraphView;
        public Type Type { get; }

        public NewGraphListView(SerializedProperty serializedPropertyList, NodeView nodeView, Type type)
        {
            SerializedProperty = serializedPropertyList;
            NodeView = nodeView;
            Type = type;

            MakeListContainer();

            this.TrackPropertyValue(serializedPropertyList, OnPropertyValueChanged);
        }

        void OnPropertyValueChanged(SerializedProperty changed)
        {
            RefreshItems();
        }

        VisualElement listElementsContainer;

        void MakeListContainer()
        {
            listElementsContainer = new();
            Add(listElementsContainer);

            // Add Button
            Button addElementBtn = new() { text = "Add Choice" };
            Add(addElementBtn);

            //addElementBtn.clicked += () => OnAddItem();

            RefreshItems();
        }

        public void RefreshItems()
        {
            for (int i = 0; i < SerializedProperty.arraySize; i++)
            {
                VisualElement itemView;
                if (i < listElementsContainer.childCount)
                {
                    itemView = listElementsContainer.ElementAt(i);
                }
                else
                {
                    itemView = MakeItem(i);
                    listElementsContainer.Add(itemView);
                }

                itemView.Unbind();
                //BindItem(itemView, i);
            }

            while (SerializedProperty.arraySize < listElementsContainer.childCount)
            {
                listElementsContainer.RemoveAt(SerializedProperty.arraySize);
            }
        }

        public VisualElement MakeItem(int i)
        {
            var listElement = new BaseGraphList();

            var propertiesInfo = Type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach(var propertyInfo in propertiesInfo)
            {
                Debug.Log($"{Type.Name} {propertyInfo.Name}");

                if (propertyInfo.IsDefined(typeof(PortAttribute), true))
                {
                    PortAttribute portAttr = propertyInfo.GetCustomAttribute<PortAttribute>();

                    Port port = PortAttribute.CreateUnbindPort(propertyInfo, NodeView);
                    if (portAttr.Direction == Direction.Input)
                    {
                        listElement.InputContainer.Add(port);
                    }
                    else
                    {
                        listElement.OutputContainer.Add(port);
                    }
                }
            }

            listElement.ContentContainer.Add(new Label() { text = $"{i}" });

            return listElement;
        }

        public void BindItem(VisualElement e,int index)
        {
            var propertiesInfo = Type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var propertyInfo in propertiesInfo)
            {
                Debug.Log($"{Type.Name} {propertyInfo.Name}");

                if (propertyInfo.IsDefined(typeof(PortAttribute), true))
                {
                    //SerializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("ports").
                }
            }
        }
        /*
        public abstract void BindItem(VisualElement e, int index);
        public abstract void OnAddItem();
        public abstract void OnRemoveItem(int index);*/

    }

    public abstract class GraphListView : VisualElement
    {
        public SerializedProperty SerializedProperty { get; }
        public NodeView NodeView { get; }
        public DialogueGraphView GraphView => NodeView.GraphView;

        public GraphListView(SerializedProperty serializedPropertyList, NodeView nodeView)
        {
            SerializedProperty = serializedPropertyList;
            NodeView = nodeView;

            MakeListContainer();

            this.TrackPropertyValue(serializedPropertyList, OnPropertyValueChanged);
        }

        void OnPropertyValueChanged(SerializedProperty changed)
        {
            RefreshItems();
        }

        VisualElement listElementsContainer;

        void MakeListContainer()
        {
            listElementsContainer = new();
            Add(listElementsContainer);

            // Add Button
            Button addElementBtn = new() { text = "Add Choice" };
            Add(addElementBtn);

            addElementBtn.clicked += () => OnAddItem();

            RefreshItems();
        }

        public void RefreshItems()
        {
            for (int i = 0; i < SerializedProperty.arraySize; i++)
            {
                VisualElement itemView;
                if (i < listElementsContainer.childCount)
                {
                    itemView = listElementsContainer.ElementAt(i);
                }
                else
                {
                    itemView = MakeItem(i);
                    listElementsContainer.Add(itemView);
                }

                itemView.Unbind();
                BindItem(itemView, i);
            }

            while (SerializedProperty.arraySize < listElementsContainer.childCount)
            {
                listElementsContainer.RemoveAt(SerializedProperty.arraySize);
            }
        }

        public abstract VisualElement MakeItem(int i);
        public abstract void BindItem(VisualElement e, int index);
        public abstract void OnAddItem();
        public abstract void OnRemoveItem(int index);
    }
}