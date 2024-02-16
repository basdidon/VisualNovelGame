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
            base.OnDrawNodeView(nodeData);
            if (nodeData is ChoicesNode choicesNode)
            {
                

                Button addCondition = new() { text = "Add Choice" };
                addCondition.clicked += () =>
                {
                    Debug.Log("a");
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

        void DrawChoicePort(Choice choice, int choiceIdx)
        {
            VisualElement ChoiceContainer = new();

            VisualElement PortsContainer = new();
            PortsContainer.style.flexDirection = FlexDirection.Row;
            PortsContainer.style.justifyContent = Justify.SpaceBetween;
            ChoiceContainer.Add(PortsContainer);

            var IsEnablePort = NodeElementFactory.GetPort<bool>(choice.IsEnableInputPortData , string.Empty, this, $"choices.Array.data[{choiceIdx}].<IsEnable>k__BackingField");
            PortsContainer.Add(IsEnablePort);

            // choice output flow port
            Port choicePort = GetOutputFlowPort(choice.OutputFlowPortData.PortGuid);
            PortsContainer.Add(choicePort);

            TextField choiceTxtField = new() { bindingPath = $"choices.Array.data[{choiceIdx}].<Name>k__BackingField" };
            choiceTxtField.value = choice.Name;
            ChoiceContainer.Add(choiceTxtField);

            Button deleteChoiceBtn = new() { text = "X" };
            ChoiceContainer.Add(deleteChoiceBtn);

            extensionContainer.Add(ChoiceContainer);

            deleteChoiceBtn.clicked += () =>
            {
                //RemovePort(isEnablePort);
                RemovePort(choicePort);
                RefreshPorts();
            };

            // Style 
            StyleLength styleLenght_8 = new(8);
            ChoiceContainer.style.marginTop = styleLenght_8;
            ChoiceContainer.style.backgroundColor = new Color(.08f, .08f, .08f, .5f);
            ChoiceContainer.style.borderBottomLeftRadius = styleLenght_8;
            ChoiceContainer.style.borderBottomRightRadius = styleLenght_8;
            ChoiceContainer.style.borderTopLeftRadius = styleLenght_8;
            ChoiceContainer.style.borderTopRightRadius = styleLenght_8;
        }
    }
}