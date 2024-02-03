using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using Graphview.NodeView;

namespace Graphview.NodeData
{
    
    public record ChoicesRecord
    {
        public DialogueRecord DialogueRecord { get; }
        public ChoicesNode.Choice Choices { get; }
        public string[] ChoicesText { get; }
        public bool[] ChoicesEnable { get; }

        public ChoicesRecord(DialogueRecord dialogueRecord, string[] choicesText, bool[] choicesEnable)
        {
            DialogueRecord = dialogueRecord;
            ChoicesText = choicesText;
            ChoicesEnable = choicesEnable;
        }

        public ChoicesRecord(CharacterData characterData, string dialogueText, string[] choicesText, bool[] choicesEnable) 
            : this(new(characterData, dialogueText), choicesText, choicesEnable) 
        { }
    }

    public class ChoicesNode : GVNodeData
    {
        [field: SerializeField] public CharacterData CharacterData { get; private set; }
        [field: SerializeField, TextArea]
        public string QuestionText { get; set; }

        [SerializeField] List<Choice> choices;
        public IReadOnlyList<Choice> Choices => choices;

        public void CreateChoice()
        {
            Choice choice = new(InstantiatePortData(Direction.Input), InstantiatePortData(Direction.Output));
            choices.Add(choice);
        }

        public void RemoveChoice(Choice choice)
        {
            choices.Remove(choice);
        }

        [System.Serializable]
        public class Choice
        {
            [SerializeField] bool isEnable;
            public bool IsEnable {
                get
                {
                    var isEnablePortConnected = false;
                    if (isEnablePortConnected)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        return isEnable;
                    }
                }
            }
            [field: SerializeField] public string Name { get; private set; }

            [field: SerializeField] public PortData IsEnableInputPortData { get; private set; }

            [field: SerializeField] public PortData OutputFlowPortData { get; private set; }

            public Choice(PortData isEnableInputPortData, PortData outputFlowPortData)
            {
                isEnable = true;
                Name = "new choice";
                IsEnableInputPortData = isEnableInputPortData;
                OutputFlowPortData = outputFlowPortData;
            }
        }
        /*
        public override void Execute(int idx)  
        {
            if (idx < 0 || idx >= Choices.Count)
            {
                throw new ArgumentOutOfRangeException();
            }
            else
            {
                Debug.Log("choices node executing");
                DialogueManager.Instance.OnSelectChoicesEvent(new(
                    CharacterData,
                    QuestionText,
                    choices.Select(c => c.Name).ToArray(),
                    choices.Select(c => c.IsEnable).ToArray()
                ));
            }
        }
        */
        [field: SerializeField] public PortData InputFlowPortData { get; private set; }

        public override void Initialize(Vector2 position, DialogueTree dialogueTree)
        {
            base.Initialize(position, dialogueTree);

            choices = new();

            SaveChanges();
        }

        public override void OnInstantiatePortData()
        {
            InputFlowPortData = InstantiatePortData(Direction.Input);
        }

    }

    [CustomGraphViewNode(typeof(ChoicesNode))]
    public class CustomChoicesGraphViewNode : GraphViewNode
    {
        public override void OnDrawNodeView(GVNodeData nodeData)
        {
            if (nodeData is ChoicesNode choicesNode)
            {
                // CharacterData ObjectField
                mainContainer.Insert(1, GetCharacterDataObjectField());

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
                for(int i = 0;i<choicesNode.Choices.Count();i++)
                {
                    DrawChoicePort(choicesNode.Choices.ElementAt(i),i);
                }

                extensionContainer.style.paddingTop = new StyleLength(4);
                extensionContainer.style.paddingBottom = new StyleLength(4);
                extensionContainer.style.paddingLeft = new StyleLength(4);
                extensionContainer.style.paddingRight = new StyleLength(4);

                // start with expanded state
                RefreshExpandedState();
            }
        }

        void DrawChoicePort(ChoicesNode.Choice choice,int choiceIdx)
        {
            VisualElement ChoiceContainer = new();
            StyleLength styleLenght_8 = new(8);
            ChoiceContainer.style.marginTop = styleLenght_8;
            ChoiceContainer.style.backgroundColor = new Color(.08f,.08f,.08f,.5f);
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

    public class ChoicesGraphData
    {
        public string Choice { get; set; }
        public bool IsEnable { get; set; }
    }
    
}