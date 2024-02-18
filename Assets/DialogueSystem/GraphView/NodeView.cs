using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace BasDidon.Dialogue.VisualGraphView
{
    public class NodeView : Node
    {
        public DialogueGraphView GraphView { get; private set; }
        public SerializedObject SerializedObject { get; private set; }

        public void Initialize(BaseNode nodeData, DialogueGraphView graphView)
        {
            GraphView = graphView;
            SetPosition(new Rect(nodeData.GraphPosition, Vector2.zero));
            viewDataKey = nodeData.Id;
            userData = nodeData;
            title = name = nodeData.GetType().Name;
            DrawHeader();
            
            SerializedObject = new(nodeData);
            mainContainer.Bind(SerializedObject);
        }

        public virtual void OnDrawNodeView(BaseNode baseNode)
        {
            Debug.Log(baseNode.GetType());

            CreatePorts(baseNode);
            CreateNodeFields(baseNode);

            RefreshExpandedState();
        }

        void CreatePorts(BaseNode baseNode)
        {
            // create port
            foreach (var pair in baseNode.Ports)
            {
                NodeElementFactory.DrawPort(pair.Value, pair.Key, this);
            }
        }

        void CreateNodeFields(BaseNode baseNode)
        {
            var nodeFieldMembers = baseNode.GetType().GetMembers(BindingFlags.Instance| BindingFlags.NonPublic| BindingFlags.Public)
                .Where(m => m.IsDefined(typeof(NodeFieldAttribute), inherit: true));

            foreach (var member in nodeFieldMembers)
            {
                var serializeProperty = SerializedObject.FindProperty(member.Name);

                if (serializeProperty == null)
                {
                    Debug.Log($"can't find {name}");
                    continue;
                }

                if (serializeProperty.propertyType == SerializedPropertyType.String)
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

        // prevent from misstyping
        public string GetPropertyBindingPath(string propertyName) => $"<{propertyName}>k__BackingField";

        void DrawHeader()
        {
            // textfield
            TextField dialogueNameTxt = new() { value = title };
            titleContainer.Insert(1, dialogueNameTxt);
            dialogueNameTxt.style.display = DisplayStyle.None;
            // Title
            VisualElement titleLabel = titleContainer.ElementAt(0);

            // double click event
            var clickable = new Clickable(ev =>
            {
                titleLabel.style.display = DisplayStyle.None;
                dialogueNameTxt.style.display = DisplayStyle.Flex;
                dialogueNameTxt.Focus();
                dialogueNameTxt.value = title;
                dialogueNameTxt.RegisterCallback<FocusOutEvent>(ev =>
                {
                    title = dialogueNameTxt.value;
                    titleLabel.style.display = DisplayStyle.Flex;
                    dialogueNameTxt.style.display = DisplayStyle.None;
                });
            });
            clickable.activators.Clear();
            clickable.activators.Add(new ManipulatorActivationFilter() { button = MouseButton.LeftMouse, clickCount = 2 });  // double click
            titleLabel.AddManipulator(clickable);
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

            //GraphView.RemoveElement(port);
        }
    }
}