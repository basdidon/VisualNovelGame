using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System;
using System.Reflection;

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

            var fields = baseNode.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach(var field in fields)
            {
                CreateNodeField(field);
                CreateSelector(field);
                CreateListElement(field);
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

        void CreateSelector(FieldInfo fieldInfo)
        {
            if (!fieldInfo.IsDefined(typeof(SelectorAttribute), inherit: true))
                return;

            EnumField enumField = new(StringHelper.ToCapitalCase(fieldInfo.Name)) { bindingPath = fieldInfo.Name };
            extensionContainer.Add(enumField);
        }
        
        void CreateNodeField(FieldInfo fieldInfo)
        {
            if (!fieldInfo.IsDefined(typeof(NodeFieldAttribute), inherit: true))
                return;

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

        void CreateListElement(FieldInfo fieldInfo)
        {
            if (fieldInfo.FieldType.IsGenericType && fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(ListElements<>))
            {
                var newList = new GraphListView(SerializedObject.FindProperty(fieldInfo.Name), this, fieldInfo.FieldType.GetGenericArguments()[0]);
                extensionContainer.Add(newList);
                RefreshExpandedState();
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
}