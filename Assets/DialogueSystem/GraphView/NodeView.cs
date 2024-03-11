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
            CreateSelectors(baseNode);
            CreateNodeFields(baseNode);

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
                EnumField enumField = new(StringHelper.ToCapitalCase(member.Name)) { bindingPath = member.Name };
                extensionContainer.Add(enumField);
            }
        }

        void CreateNodeFields(BaseNode baseNode)
        {
            Type[] types = new[] { typeof(string), typeof(int) ,typeof(UnityEngine.Object)};

            var nodeFields = baseNode.GetType().GetFields(BindingFlags.Instance| BindingFlags.NonPublic| BindingFlags.Public)
                .Where(m => m.IsDefined(typeof(NodeFieldAttribute), inherit: true));

            foreach (var field in nodeFields)
            {
                var serializeProperty = SerializedObject.FindProperty(field.Name);

                if (serializeProperty == null)
                {
                    Debug.Log($"can't find {name}");
                    continue;
                }

                Type fieldType = GetFieldOrPropertyType(baseNode.GetType(),field.Name);

                if (types.Contains(fieldType))
                {
                    var propField = new PropertyField(serializeProperty);
                    extensionContainer.Add(propField);
                }
                else if(types.Any(t=> fieldType.IsSubclassOf(t)))
                {
                    var propField = new PropertyField(serializeProperty);
                    extensionContainer.Add(propField);
                }
                else
                {
                    throw new System.InvalidOperationException();
                }
            }

        }

        void CreateListFields(BaseNode baseNode)
        {
            var ListFields = baseNode.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(f => f.IsDefined(typeof(ListFieldAttribute), inherit: true));


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