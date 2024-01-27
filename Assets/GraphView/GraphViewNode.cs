using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Graphview.NodeData;

namespace Graphview.NodeView
{
    public abstract class GraphViewNode : Node
    {
        public GVNodeData NodeData { get; private set; }

        public void Initialize(GVNodeData nodeData)
        {
            NodeData = nodeData;
            SetPosition(new Rect(nodeData.GraphPosition, Vector2.zero));
            viewDataKey = nodeData.Id;
            userData = nodeData;
            title = name = nodeData.GetType().Name;
            DrawHeader();
        }

        public abstract void OnDrawNodeView(GVNodeData nodeData);

        public Port GetInputFlowPort()
        {
            // input port
            Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(ExecutionFlow));
            inputPort.portName = "input";
            inputPort.portColor = Color.yellow;
            return inputPort;
        }

        public Port GetOutputFlowPort()
        {
            // input port
            Port outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(ExecutionFlow));
            outputPort.portName = "output";
            outputPort.portColor = Color.yellow;
            return outputPort;
        }

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


    }
}