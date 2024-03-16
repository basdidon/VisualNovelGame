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

            var fields = baseNode.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach(var field in fields)
            {
                if(field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(ListElements<>))
                {
                    var newList = new GraphListView(SerializedObject.FindProperty(field.Name), this, field.FieldType.GetGenericArguments()[0]);
                    extensionContainer.Add(newList);
                    RefreshExpandedState();

                }
            }
            RefreshExpandedState();
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
            foreach (var portData in baseNode.Ports)
            {
                PropertyInfo property = baseNode.GetType().GetProperty(portData.FieldName);
                if (property == null)
                    continue;

                if (property.IsDefined(typeof(PortAttribute)))
                {
                    var portAttr = property.GetCustomAttribute<PortAttribute>();
                    VisualElement portContainer = portData.Direction == Direction.Input ? inputContainer : outputContainer;
                    Port port = portAttr.CreatePort(property, this, portData,SerializedObject);

                    portContainer.Add(port);
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

        public void AddPort(Port portToAdd)
        {
            if(portToAdd.direction == Direction.Input)
            {
                InputContainer.Add(portToAdd);
            }
            else
            {
                OutputContainer.Add(portToAdd);
            }
        }
    }

    public class GraphListView : VisualElement
    {
        public SerializedProperty SerializedProperty { get; }
        public NodeView NodeView { get; }
        public DialogueGraphView GraphView => NodeView.GraphView;
        public Type Type { get; }

        public GraphListView(SerializedProperty serializedPropertyList, NodeView nodeView, Type type)
        {
            SerializedProperty = serializedPropertyList.FindPropertyRelative("listElements");
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
                BindItem(itemView, i);
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
                if (propertyInfo.IsDefined(typeof(PortAttribute), true))
                {
                    var portAttr = propertyInfo.GetCustomAttribute<PortAttribute>();
                    Port port = portAttr.CreateUnbindPort(propertyInfo, NodeView);
                    listElement.AddPort(port);
                    
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
                var portAttr = propertyInfo.GetCustomAttribute<PortAttribute>();

                if (propertyInfo.IsDefined(typeof(PortAttribute), true))
                {
                    var itemSP = SerializedProperty.GetArrayElementAtIndex(index);
                    var portsSP = itemSP.FindPropertyRelative("portCollection");
                    var portCollectionSP = portsSP.FindPropertyRelative("portList");
                    Debug.Log($"------[ {propertyInfo.Name} {portCollectionSP.isArray} ]-------");

                    if (portCollectionSP.isArray)
                    {
                        Debug.Log(portCollectionSP.arraySize);
                        for (int i = 0; i < portCollectionSP.arraySize; i++)
                        {
                            var portData = portCollectionSP.GetArrayElementAtIndex(i);
                            var fieldName = portData.FindPropertyRelative("<FieldName>k__BackingField").stringValue;
                            Debug.Log(fieldName);
                            if (fieldName == propertyInfo.Name)
                            {
                                var portGuid= portData.FindPropertyRelative("<PortGuid>k__BackingField").stringValue;

                                Debug.Log($"<color=blue>Binding {SerializedProperty.name}[{index}].{fieldName}</color> {portGuid}");
                                var portFactory = PortFactoryUtils.GetPortFactory(propertyInfo.PropertyType);
                                if (portAttr.HasBackingFieldName)
                                {
                                    var backingFieldSP = itemSP.FindPropertyRelative(portAttr.BackingFieldName);
                                    portFactory.BindPort(e,fieldName, portGuid, portAttr, backingFieldSP);
                                }
                                else
                                {
                                    portFactory.BindPort(e,fieldName,portGuid,portAttr);
                                }
                            }
                        }

                    }
                }
            }
        }
        /*
        public abstract void BindItem(VisualElement e, int index);
        public abstract void OnAddItem();
        public abstract void OnRemoveItem(int index);*/

    }
}