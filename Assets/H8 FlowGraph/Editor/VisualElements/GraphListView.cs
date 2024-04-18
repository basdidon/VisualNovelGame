using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Reflection;

namespace H8.FlowGraph.UiElements
{
    public class GraphListView : VisualElement
    {
        public SerializedProperty SerializedProperty { get; }
        public NodeView NodeView { get; }
        public DialogueGraphView GraphView => NodeView.GraphView;
        public Type Type { get; }

        int lastArraySize;

        public GraphListView(SerializedProperty serializedPropertyList, NodeView nodeView, Type type)
        {
            SerializedProperty = serializedPropertyList.FindPropertyRelative("listElements");
            NodeView = nodeView;
            Type = type;

            MakeListContainer();
            
            lastArraySize = SerializedProperty.arraySize;// serializedPropertyList.FindPropertyRelative("listElements").arraySize;
            this.TrackPropertyValue(SerializedProperty, OnPropertyValueChanged);
        }

        void OnPropertyValueChanged(SerializedProperty changed)
        {
            if (changed.arraySize != lastArraySize)
            {
                lastArraySize = changed.arraySize;
                RefreshItems();
            }
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

        public VisualElement MakeItem(int i)
        {
            var listElement = new BaseGraphList();

            var propertiesInfo = Type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach(var propertyInfo in propertiesInfo)
            {
                if (propertyInfo.IsDefined(typeof(PortAttribute), true))
                {
                    var portAttr = propertyInfo.GetCustomAttribute<PortAttribute>();
                    Port port = PortFactory.CreateUnbindPort(portAttr,propertyInfo, NodeView);
                    listElement.AddPort(port);
                }
            }

            var fieldsInfo = Type.GetFields();
            foreach(var fieldInfo in fieldsInfo)
            {
                if (fieldInfo.IsDefined(typeof(NodeFieldAttribute), true))
                {
                    var portAttr = fieldInfo.GetCustomAttribute<NodeFieldAttribute>();
                    PropertyField propertyField = portAttr.CreateUnbindPropertyField(fieldInfo);
                    listElement.ContentContainer.Add(propertyField);
                }
            }

            Button removeElementBtn = new() { text = $"Remove ({i})" };
            listElement.contentContainer.Add(removeElementBtn);

            removeElementBtn.clicked += () => OnRemoveItem(i);
            return listElement;
        }

        public void BindItem(VisualElement e,int index)
        {
            var propertiesInfo = Type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var itemSP = SerializedProperty.GetArrayElementAtIndex(index);

            foreach (var propertyInfo in propertiesInfo)
            {
                var portAttr = propertyInfo.GetCustomAttribute<PortAttribute>();

                if (propertyInfo.IsDefined(typeof(PortAttribute), true))
                {
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

            foreach(var fieldinfo in Type.GetFields())
            {
                if (fieldinfo.IsDefined(typeof(NodeFieldAttribute), inherit: true))
                {
                    var fieldSP = itemSP.FindPropertyRelative(fieldinfo.Name);
                    if (fieldSP == null)
                        throw new Exception();

                    var propertyField = e.Q<PropertyField>(fieldinfo.Name);
                    propertyField.BindProperty(fieldSP);
                }
            }
        }
        public void OnAddItem()
        {
            Debug.Log("OnAddItem");
            Debug.Log(SerializedProperty.arraySize);
            SerializedProperty.InsertArrayElementAtIndex(SerializedProperty.arraySize);
            Debug.Log(SerializedProperty.arraySize);

            SerializedProperty.serializedObject.ApplyModifiedProperties();
            
        }

        public void OnRemoveItem(int index)
        {
            SerializedProperty.DeleteArrayElementAtIndex(index);
            SerializedProperty.serializedObject.ApplyModifiedProperties();
        }

    }
}