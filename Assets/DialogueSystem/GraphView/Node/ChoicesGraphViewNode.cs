using UnityEngine;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace BasDidon.Dialogue.VisualGraphView
{
    [CustomGraphViewNode(typeof(ChoicesNode))]
    public class ChoicesGraphViewNode : GraphViewNode
    {
        public override void OnDrawNodeView(BaseNode nodeData)
        {
            if (nodeData is ChoicesNode choicesNode)
            {
                // CharacterData ObjectField
                TextField speakerTextField = new()
                {
                    label = "Speaker",
                    bindingPath = GetPropertyBindingPath("SpeakerName"),
                };
                mainContainer.Insert(1, speakerTextField);

                var inputFlowPort = GetInputFlowPort(choicesNode.InputFlowPortData.PortGuid);
                inputContainer.Add(inputFlowPort);

                Button addCondition = new() { text = "Add Choice" };
                addCondition.clicked += () =>
                {
                    choicesNode.CreateChoice();

                    DrawChoicePort(choicesNode.Choices.Last(), choicesNode.Choices.Count() - 1);
                    RefreshExpandedState();
                };
                mainContainer.Insert(2, addCondition);

                // output port
                for (int i = 0; i < choicesNode.Choices.Count(); i++)
                {
                    DrawChoicePort(choicesNode.Choices.ElementAt(i), i);
                }

                extensionContainer.style.paddingTop = new StyleLength(4);
                extensionContainer.style.paddingBottom = new StyleLength(4);
                extensionContainer.style.paddingLeft = new StyleLength(4);
                extensionContainer.style.paddingRight = new StyleLength(4);

                // start with expanded state
                RefreshExpandedState();
            }
        }

        void DrawChoicePort(ChoicesNode.Choice choice, int choiceIdx)
        {
            VisualElement ChoiceContainer = new();
            StyleLength styleLenght_8 = new(8);
            ChoiceContainer.style.marginTop = styleLenght_8;
            ChoiceContainer.style.backgroundColor = new Color(.08f, .08f, .08f, .5f);
            ChoiceContainer.style.borderBottomLeftRadius = styleLenght_8;
            ChoiceContainer.style.borderBottomRightRadius = styleLenght_8;
            ChoiceContainer.style.borderTopLeftRadius = styleLenght_8;
            ChoiceContainer.style.borderTopRightRadius = styleLenght_8;

            VisualElement PortsContainer = new();
            PortsContainer.style.flexDirection = FlexDirection.Row;
            PortsContainer.style.justifyContent = Justify.SpaceBetween;
            ChoiceContainer.Add(PortsContainer);

            Port isEnablePort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            isEnablePort.viewDataKey = choice.IsEnableInputPortData.PortGuid;
            isEnablePort.portName = string.Empty;
            //isEnablePort.userData;
            PortsContainer.Add(isEnablePort);

            var isEnableToggle = new Toggle() { bindingPath = $"choices.Array.data[{choiceIdx}].isEnable" };
            isEnablePort.Add(isEnableToggle);

            // choice output flow port
            Port choicePort = GetOutputFlowPort(choice.OutputFlowPortData.PortGuid);
            PortsContainer.Add(choicePort);

            TextField choiceTxtField = new() { bindingPath = $"choices.Array.data[{choiceIdx}].<Name>k__BackingField" };
            ChoiceContainer.Add(choiceTxtField);

            Button deleteChoiceBtn = new() { text = "X" };
            ChoiceContainer.Add(deleteChoiceBtn);

            extensionContainer.Add(ChoiceContainer);

            deleteChoiceBtn.clicked += () =>
            {
                RemovePort(isEnablePort);
                //RemovePort(choicePort);
                RefreshPorts();
            };
        }
    }
}