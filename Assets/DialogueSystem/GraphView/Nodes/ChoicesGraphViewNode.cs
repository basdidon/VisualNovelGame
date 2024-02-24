using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;
using System;
using UnityEditor.Experimental.GraphView;

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
                var serializedChoices = SerializedObject.FindProperty("choices");
                if (serializedChoices.isArray)
                {
                    for (int i = 0; i < serializedChoices.arraySize; i++)
                    {
                        Debug.Log(i);
                        DrawChoicePort(serializedChoices.GetArrayElementAtIndex(i));
                    }

                }

                Button addCondition = new() { text = "Add Choice" };
                addCondition.clicked += () =>
                {
                    choicesNode.CreateChoice();
                    serializedChoices.serializedObject.Update();
                    var choiceProperty = serializedChoices.GetArrayElementAtIndex(serializedChoices.arraySize - 1);

                    DrawChoicePort(choiceProperty);
                    RefreshExpandedState();
                };
                mainContainer.Insert(2, addCondition);



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

                SerializedObject.ApplyModifiedProperties();
            }
        }
        /*
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
        */
        void DrawChoicePort(SerializedProperty serializedChoice)
        {
            VisualElement ChoiceContainer = new();

            VisualElement PortsContainer = new();
            ChoiceContainer.Add(PortsContainer);

            serializedChoice.isExpanded = true;

            var isEnableInputPortSP = serializedChoice.FindPropertyRelative("<IsEnableInputPortData>k__BackingField");
            string isEnablePortGuid = isEnableInputPortSP.FindPropertyRelative("<PortGuid>k__BackingField").stringValue;
                
            var IsEnablePort = NodeElementFactory.CreatePortWithField(
                serializedChoice.FindPropertyRelative("<IsEnable>k__BackingField"),
                typeof(bool),
                isEnablePortGuid,
                Direction.Input,
                this,
                ""
            );

            PortsContainer.Add(IsEnablePort);

            //
            var outputFlowPortSP = serializedChoice.FindPropertyRelative("<OutputFlowPortData>k__BackingField");
            string outputFlowPortGuid = outputFlowPortSP.FindPropertyRelative("<PortGuid>k__BackingField").stringValue;

            var outputFlowPort = NodeElementFactory.CreatePortWithField(
                serializedChoice.FindPropertyRelative("<Output>k_BackingField"),
                typeof(ExecutionFlow),
                outputFlowPortGuid,
                Direction.Output,
                this,
                "Output"
            );

            PortsContainer.Add(outputFlowPort);

            var nameSP = serializedChoice.FindPropertyRelative("<Name>k__BackingField");
            var choiceText = new PropertyField(nameSP,string.Empty);// TextField() { bindingPath = $"choices.Array.data[{choiceIdx}].<Name>k__BackingField" };
            choiceText.BindProperty(nameSP);

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