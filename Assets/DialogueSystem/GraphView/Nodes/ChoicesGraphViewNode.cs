using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;
using System;

namespace BasDidon.Dialogue.VisualGraphView
{
    [CustomGraphViewNode(typeof(ChoicesNode))]
    public class ChoicesGraphViewNode : NodeView
    {
        public override void OnDrawNodeView(BaseNode nodeData)
        {
            base.OnDrawNodeView(nodeData);
            if (nodeData is ChoicesNode choicesNode)
            {
                Button addCondition = new() { text = "Add Choice" };
                addCondition.clicked += () =>
                {
                    choicesNode.CreateChoice();

                    DrawChoicePort(choicesNode.Choices.Last(), choicesNode.Choices.Count() - 1);
                    RefreshExpandedState();
                };
                mainContainer.Insert(2, addCondition);

                var serializedChoices = SerializedObject.FindProperty("choices");
                if (serializedChoices.isArray)
                {
                    for(int i = 0; i < serializedChoices.arraySize; i++)
                    {
                        Debug.Log(i);
                        var serializedQuestionText = serializedChoices.GetArrayElementAtIndex(i).FindPropertyRelative("<Name>k__BackingField");
                        if(serializedQuestionText != null)
                            Debug.Log(serializedQuestionText.stringValue);

                        DrawChoicePort(serializedChoices.GetArrayElementAtIndex(i));
                    }
                }

                // output port
                /*
                for (int i = 0; i < choicesNode.Choices.Count(); i++)
                {
                    DrawChoicePort(choicesNode.Choices.ElementAt(i), i);
                }*/

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
            ChoiceContainer.Add(PortsContainer);

            var IsEnablePort = NodeElementFactory.CreatePort(typeof(bool),choice.IsEnableInputPortData, $"choices.Array.data[{choiceIdx}].<IsEnable>k__BackingField", this);
            PortsContainer.Add(IsEnablePort);

            // choice output flow port
            var choicePort = NodeElementFactory.CreatePort(typeof(ExecutionFlow),choice.OutputFlowPortData, string.Empty, this);
            PortsContainer.Add(choicePort);

            var choiceText = new TextField() { bindingPath = $"choices.Array.data[{choiceIdx}].<Name>k__BackingField" };
            ChoiceContainer.Add(choiceText);

            Button deleteChoiceBtn = new() { text = "X" };
            ChoiceContainer.Add(deleteChoiceBtn);

            extensionContainer.Add(ChoiceContainer);

            deleteChoiceBtn.clicked += () =>
            {
                //RemovePort(isEnablePort);
                //RemovePort(choicePort);
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

            PortsContainer.style.flexDirection = FlexDirection.Row;
            PortsContainer.style.justifyContent = Justify.SpaceBetween;
        }

        void DrawChoicePort(SerializedProperty serializedChoice)
        {
            VisualElement ChoiceContainer = new();

            VisualElement PortsContainer = new();
            ChoiceContainer.Add(PortsContainer);
            /*
            var IsEnablePort = NodeElementFactory.CreatePortWithField(
                serializedChoice.FindPropertyRelative("<IsEnable>k__BackingField"),
                typeof(bool),
                //new( serializedChoice.FindPropertyRelative("<IsEnableInputPortData>k__BackingField").FindPropertyRelative("<PortGuid>k__BackingField"),
                this
            );
            /*
            var IsEnablePort = NodeElementFactory.CreatePort(typeof(bool), choice.IsEnableInputPortData, $"choices.Array.data[{choiceIdx}].<IsEnable>k__BackingField", this);
            PortsContainer.Add(IsEnablePort);

            // choice output flow port
            var choicePort = NodeElementFactory.CreatePort(typeof(ExecutionFlow), choice.OutputFlowPortData, string.Empty, this);
            PortsContainer.Add(choicePort);
            */
            var choiceText = new PropertyField(serializedChoice.FindPropertyRelative("<Name>k__BackingField"));// TextField() { bindingPath = $"choices.Array.data[{choiceIdx}].<Name>k__BackingField" };
            ChoiceContainer.Add(choiceText);

            Button deleteChoiceBtn = new() { text = "X" };
            ChoiceContainer.Add(deleteChoiceBtn);

            extensionContainer.Add(ChoiceContainer);

            deleteChoiceBtn.clicked += () =>
            {
                //RemovePort(isEnablePort);
                //RemovePort(choicePort);
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

            PortsContainer.style.flexDirection = FlexDirection.Row;
            PortsContainer.style.justifyContent = Justify.SpaceBetween;
        }
    }
}