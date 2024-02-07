using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Graphview.NodeData;

namespace Graphview.NodeView
{
    public abstract class GraphViewNode : Node
    {
        public DialogueGraphView GraphView { get; private set; }
        public SerializedObject SerializedObject { get; private set; }

        public void Initialize(NodeData.NodeData nodeData, DialogueGraphView graphView)
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

        public abstract void OnDrawNodeView(NodeData.NodeData nodeData);

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

        public ObjectField GetCharacterDataObjectField(string propertyName = "CharacterData")
        {
            ObjectField characterDataObjectField = new()
            {
                objectType = typeof(CharacterData),
                bindingPath = GetPropertyBindingPath(propertyName),
            };
            return characterDataObjectField;
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