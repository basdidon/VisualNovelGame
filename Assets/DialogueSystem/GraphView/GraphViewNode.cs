using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System.Reflection;

namespace BasDidon.Dialogue.VisualGraphView
{
    public class GraphViewNode : Node
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
            // create port
            foreach (var pair in baseNode.Ports)
            {
                var property = baseNode.GetType().GetProperty(pair.Key);
                var port = NodePortFactory.GetPort(property.PropertyType, pair.Value, property.Name, this);

                if (pair.Value.Direction == Direction.Input)
                {
                    inputContainer.Add(port);
                }
                else
                {
                    outputContainer.Add(port);
                }

                Debug.Log($"<color=blue>DrawPort</color> {pair.Value.Direction} ({property.PropertyType.Name})");
            }

            var nodeFieldProperties = baseNode.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(p => p.IsDefined(typeof(NodeFieldAttribute), inherit: false));

            Debug.Log(nodeFieldProperties.Count());
            foreach (var property in nodeFieldProperties)
            {
                var propertyBindingPath = GetPropertyBindingPath(property.Name);
                var serializeProperty = SerializedObject.FindProperty(propertyBindingPath);
                Debug.Log($"{property.Name} : {serializeProperty.stringValue}");
                var propField = new PropertyField(serializeProperty);
                extensionContainer.Add(propField);
                /*
                if(property.PropertyType == typeof(string))
                {
                    TextField speakerTextField = new()
                    {
                        label = "Speaker",
                        bindingPath = GetPropertyBindingPath("SpeakerName"),
                    };
                    speakerTextField.AddToClassList("speaker-name-input");
                    extensionContainer.Add(speakerTextField);
                }*/
            }



            RefreshExpandedState();
        }

        public Port GetInputFlowPort(string guid)
        {
            // input port
            Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(ExecutionFlow));
            inputPort.portName = "input";
            inputPort.portColor = Color.yellow;
            inputPort.viewDataKey = guid;
            return inputPort;
        }

        public Port GetOutputFlowPort(string guid)
        {
            // input port
            Port outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(ExecutionFlow));
            outputPort.portName = "output";
            outputPort.portColor = Color.yellow;
            outputPort.viewDataKey = guid;
            return outputPort;
        }

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