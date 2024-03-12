using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;
using System;
using UnityEditor.Experimental.GraphView;

namespace BasDidon.Dialogue.NodeTemplate
{
    using VisualGraphView;

    public class ChoicesGraphListView : GraphListView
    {
        public ChoicesGraphListView(SerializedProperty serializedProperty,NodeView nodeView) : base(serializedProperty, nodeView)
        {

        }

        public override VisualElement MakeItem(int i)
        {
            VisualElement listElement = new();
            listElement.style.flexDirection = FlexDirection.Column;

            VisualElement PortsContainer = new();
            listElement.Add(PortsContainer);

            // IsEnablePort
            var IsEnablePort = NodeView.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            IsEnablePort.name = "is_enable_port";
            IsEnablePort.portName = string.Empty;
            PortsContainer.Add(IsEnablePort);

            // IsEnableToggle
            Toggle IsEnableToggle = new()
            {
                name = "is_enable_toggle"
            };
            IsEnablePort.Add(IsEnableToggle);

            GraphView.OnPortConnect += port =>
            {
                if (port.viewDataKey == IsEnablePort.viewDataKey)
                {
                    IsEnableToggle.style.display = DisplayStyle.None;
                }
            };

            GraphView.OnPortDisconnect += port =>
            {
                if (port.viewDataKey == IsEnablePort.viewDataKey)
                {
                    IsEnableToggle.style.display = DisplayStyle.Flex;
                }
            };

            // OutputFlowPort
            var outputFlowPort = NodeView.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(ExecutionFlow));
            outputFlowPort.name = "output_flow_port";
            outputFlowPort.portName = string.Empty;
            outputFlowPort.portColor = Color.yellow;
            PortsContainer.Add(outputFlowPort);

            // ChoiceText
            var choiceText = new TextField()
            {
                name = "choice_text"
            };
            listElement.Add(choiceText);

            // DeleteChoiceButton
            Button deleteChoiceBtn = new()
            {
                text = "X",
                name = "delete_choice_btn"
            };
            listElement.Add(deleteChoiceBtn);

            deleteChoiceBtn.clicked += () => OnRemoveItem(i);
            deleteChoiceBtn.text = $"X {i}";

            // Style 
            style.marginTop = new(2f);
            style.marginBottom = new(0f);
            style.marginRight = new(0f);
            style.marginLeft = new(0f);
            style.backgroundColor = new Color(.08f, .08f, .08f, .5f);
            style.borderBottomLeftRadius = new(8);
            style.borderBottomRightRadius = new(8);
            style.borderTopLeftRadius = new(8);
            style.borderTopRightRadius = new(8);

            PortsContainer.style.flexDirection = FlexDirection.Row;
            PortsContainer.style.justifyContent = Justify.SpaceBetween;

            return listElement;
        }

        public override void BindItem(VisualElement e, int index)
        {
            var SerializedChoice = SerializedProperty.GetArrayElementAtIndex(index);

            var isEnableInputPortSP = SerializedChoice.FindPropertyRelative("<IsEnableInputPortData>k__BackingField");
            string isEnablePortGuid = isEnableInputPortSP.FindPropertyRelative("<PortGuid>k__BackingField").stringValue;
            var isEnablePort = e.Q<Port>("is_enable_port");
            isEnablePort.viewDataKey = isEnablePortGuid;

            var isEnableSP = SerializedChoice.FindPropertyRelative("<IsEnable>k__BackingField");
            var isEnableToggle = e.Q<Toggle>("is_enable_toggle");
            isEnableToggle.BindProperty(isEnableSP);

            var outputFlowPortSP = SerializedChoice.FindPropertyRelative("<OutputFlowPortData>k__BackingField");
            string outputFlowPortGuid = outputFlowPortSP.FindPropertyRelative("<PortGuid>k__BackingField").stringValue;
            var outputFlowPort = e.Q<Port>("output_flow_port");
            outputFlowPort.viewDataKey = outputFlowPortGuid;

            // ChoiceText
            var choiceTextSP = SerializedChoice.FindPropertyRelative("<Name>k__BackingField");
            var choiceText = e.Q<TextField>("choice_text");
            choiceText.BindProperty(choiceTextSP);
        }


        public override void OnAddItem()
        {
            SerializedProperty.InsertArrayElementAtIndex(SerializedProperty.arraySize);
            var serializedChoice = SerializedProperty.GetArrayElementAtIndex(SerializedProperty.arraySize - 1);

            var isEnableInputPortSP = serializedChoice.FindPropertyRelative("<IsEnableInputPortData>k__BackingField");
            


            //ChoicesNode.CreateChoice();
        }

        public override void OnRemoveItem(int index)
        {
            //ChoicesNode.RemoveChoiceAt(index);
        }
    }

    /*
    [CustomGraphViewNode(typeof(ChoicesNode))]
    public class ChoicesGraphViewNode : NodeView
    {
        public override void OnDrawNodeView(BaseNode nodeData)
        {
            base.OnDrawNodeView(nodeData);
            if (nodeData is ChoicesNode choicesNode)
            {
                GraphListView listView = new (SerializedObject.FindProperty("choices"), this,MakeItem,BindItem,choicesNode.CreateChoice,choicesNode.RemoveChoiceAt);
                extensionContainer.Add(listView);

                extensionContainer.style.paddingTop = new StyleLength(4);
                extensionContainer.style.paddingBottom = new StyleLength(4);
                extensionContainer.style.paddingLeft = new StyleLength(4);
                extensionContainer.style.paddingRight = new StyleLength(4);

                // start with expanded state
                RefreshExpandedState();

                SerializedObject.ApplyModifiedProperties();
            }
        }

        public VisualElement MakeItem(Action<int> deleteAction,int i)
        {
            VisualElement listElement = new();
            listElement.style.flexDirection = FlexDirection.Column;

            VisualElement PortsContainer = new();
            listElement.Add(PortsContainer);

            // IsEnablePort
            var IsEnablePort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            IsEnablePort.name = "is_enable_port";
            IsEnablePort.portName = string.Empty;
            PortsContainer.Add(IsEnablePort);

            // IsEnableToggle
            Toggle IsEnableToggle = new()
            {
                name = "is_enable_toggle"
            };
            IsEnablePort.Add(IsEnableToggle);

            GraphView.OnPortConnect += port =>
            {
                if(port.viewDataKey == IsEnablePort.viewDataKey)
                {
                    IsEnableToggle.style.display = DisplayStyle.None;
                }
            };

            GraphView.OnPortDisconnect += port =>
            {
                if (port.viewDataKey == IsEnablePort.viewDataKey)
                {
                    IsEnableToggle.style.display = DisplayStyle.Flex;
                }
            };

            // OutputFlowPort
            var outputFlowPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(ExecutionFlow));
            outputFlowPort.name = "output_flow_port";
            outputFlowPort.portName = string.Empty;
            outputFlowPort.portColor = Color.yellow;
            PortsContainer.Add(outputFlowPort);

            // ChoiceText
            var choiceText = new TextField() {
                name = "choice_text"
            };
            listElement.Add(choiceText);

            // DeleteChoiceButton
            Button deleteChoiceBtn = new() { 
                text = "X", 
                name = "delete_choice_btn"
            };
            listElement.Add(deleteChoiceBtn);

            deleteChoiceBtn.clicked += () => deleteAction(i);
            deleteChoiceBtn.text = $"X {i}";

            // Style 
            style.marginTop = new(2f);
            style.marginBottom = new(0f);
            style.marginRight = new(0f);
            style.marginLeft = new(0f);
            style.backgroundColor = new Color(.08f, .08f, .08f, .5f);
            style.borderBottomLeftRadius = new(8);
            style.borderBottomRightRadius = new(8);
            style.borderTopLeftRadius = new(8);
            style.borderTopRightRadius = new(8);

            PortsContainer.style.flexDirection = FlexDirection.Row;
            PortsContainer.style.justifyContent = Justify.SpaceBetween;
            
            return listElement;
        }

        public void BindItem(VisualElement visualElement,int index)
        {
            var SerializedChoice = SerializedObject.FindProperty("choices").GetArrayElementAtIndex(index);

            var isEnableInputPortSP = SerializedChoice.FindPropertyRelative("<IsEnableInputPortData>k__BackingField");
            string isEnablePortGuid = isEnableInputPortSP.FindPropertyRelative("<PortGuid>k__BackingField").stringValue;
            var isEnablePort = visualElement.Q<Port>("is_enable_port");
            isEnablePort.viewDataKey = isEnablePortGuid;

            var isEnableSP = SerializedChoice.FindPropertyRelative("<IsEnable>k__BackingField");
            var isEnableToggle = visualElement.Q<Toggle>("is_enable_toggle");
            isEnableToggle.BindProperty(isEnableSP);

            var outputFlowPortSP = SerializedChoice.FindPropertyRelative("<OutputFlowPortData>k__BackingField");
            string outputFlowPortGuid = outputFlowPortSP.FindPropertyRelative("<PortGuid>k__BackingField").stringValue;
            var outputFlowPort = visualElement.Q<Port>("output_flow_port");
            outputFlowPort.viewDataKey = outputFlowPortGuid;

            // ChoiceText
            var choiceTextSP = SerializedChoice.FindPropertyRelative("<Name>k__BackingField");
            var choiceText = visualElement.Q<TextField>("choice_text");
            choiceText.BindProperty(choiceTextSP);
        }
    }*/
}