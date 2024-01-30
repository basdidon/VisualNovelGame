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

        public void AddChoice(Choice choice)
        {
            choices.Add(choice);
        }

        public void RemoveChoice(Choice choice)
        {
            choices.Remove(choice);
        }

        [System.Serializable]
        public class Choice
        {
            [field:SerializeField] public ChoicesNode ChoicesNode { get; private set; }
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

            [SerializeField] PortData isEnableInputPortData;
            public PortData IsEnableInputPortData => isEnableInputPortData;

            [SerializeField] PortData outputFlowPortData;
            public PortData OutputFlowPortData => outputFlowPortData;

            public Choice(ChoicesNode choicesNode)
            {
                ChoicesNode = choicesNode;
                isEnable = true;
                Name = "new choice";
                isEnableInputPortData = new(ChoicesNode.DialogueTree, Direction.Input);
                outputFlowPortData = new(ChoicesNode.DialogueTree,Direction.Output);
            }
        }

        public override void Execute()
        {
            DialogueManager.Instance.OnSelectChoicesEvent(new(
                CharacterData, 
                QuestionText, 
                choices.Select(c => c.Name).ToArray(),
                choices.Select(c=>c.IsEnable).ToArray()
            ));
        }

        [SerializeField] PortData inputFlowPortData;
        public PortData InputFlowPortData => inputFlowPortData;

        public override string[] InputPortGuids => choices.Select(choice=> choice.IsEnableInputPortData.PortGuid).Append(InputFlowPortData.PortGuid).ToArray();
        public override string[] OutputPortGuids => choices.Select(choice => choice.OutputFlowPortData.PortGuid).ToArray();

        public override void Initialize(Vector2 position, DialogueTree dialogueTree)
        {
            base.Initialize(position, dialogueTree);
            inputFlowPortData = new(dialogueTree, Direction.Input);
            choices = new();

            SaveChanges();
        }


        public override IEnumerable<GVNodeData> GetChildren() => Choices.SelectMany(c=>c.OutputFlowPortData.ConnectedNode);
    }

    [CustomGraphViewNode(typeof(ChoicesNode))]
    public class CustomChoicesGraphViewNode : GraphViewNode
    {
        public override void OnDrawNodeView(GVNodeData nodeData)
        {
            if (nodeData is ChoicesNode choicesNode)
            {
                SerializedObject SO = new(choicesNode);
                mainContainer.Bind(SO);

                var inputFlowPort = GetInputFlowPort(choicesNode.InputFlowPortData.PortGuid);
                inputContainer.Add(inputFlowPort);

                Button addCondition = new() { text = "Add Choice" };
                addCondition.clicked += () => OnAddChoice(choicesNode);
                mainContainer.Insert(1, addCondition);

                choicesNode.QuestionText ??= string.Empty;

                // CharacterData ObjectField
                mainContainer.Insert(1,GetCharacterDataObjectField());

                // Custom extension
                VisualElement customVisualElement = new();

                var textArea = new TextField()
                {
                    bindingPath = GetPropertyBindingPath("QuestionText"),
                    multiline = true,
                };
                customVisualElement.Add(textArea);

                extensionContainer.Add(customVisualElement);

                // output port
                for(int i = 0;i<choicesNode.Choices.Count();i++)
                {
                    DrawChoicePort(choicesNode.Choices.ElementAt(i),i);
                }

                // start with expanded state
                RefreshExpandedState();
            }
        }

        void DrawChoicePort(ChoicesNode.Choice choice,int choiceIdx)
        {
            VisualElement ChoiceContainer = new();
            ChoiceContainer.style.marginTop = new StyleLength(8);
            ChoiceContainer.style.backgroundColor = Color.cyan;

            VisualElement PortsContainer = new();
            PortsContainer.style.flexDirection = FlexDirection.Row;
            PortsContainer.style.justifyContent = Justify.SpaceBetween;
            ChoiceContainer.Add(PortsContainer);

            Port isEnablePort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            isEnablePort.viewDataKey = choice.OutputFlowPortData.PortGuid;
            isEnablePort.portName = "IsEnable";
            
            var isEnablePortLabel =  isEnablePort.Q<Label>();
            isEnablePortLabel.style.color = Color.black;
            
            PortsContainer.Add(isEnablePort);

            var isEnableToggle = new Toggle() { bindingPath = $"choices.Array.data[{choiceIdx}].isEnable" };
            isEnablePort.Add(isEnableToggle);

            // choice output flow port
            Port choicePort = InstantiatePort(Orientation.Horizontal,Direction.Output,Port.Capacity.Single,typeof(ExecutionFlow));
            choicePort.portName = string.Empty;
            choicePort.viewDataKey = choice.OutputFlowPortData.PortGuid;
            Debug.Log($"{choicePort.viewDataKey} {choicePort.direction}");
            PortsContainer.Add(choicePort);

            TextField choiceTxtField = new() { bindingPath = $"choices.Array.data[{choiceIdx}].<Name>k__BackingField" };
            ChoiceContainer.Add(choiceTxtField);
            
            Button deleteChoiceBtn = new() { text = "X" };
            ChoiceContainer.Add(deleteChoiceBtn);

            extensionContainer.Add(ChoiceContainer);

            deleteChoiceBtn.clicked += () =>
            {
                RemovePort(isEnablePort);
                RemovePort(choicePort);
                RefreshPorts();
                
            };
        }

        void OnAddChoice(ChoicesNode choicesNode)
        {
            ChoicesNode.Choice choice = new(choicesNode);
            choicesNode.AddChoice(choice);

            DrawChoicePort(choice, choicesNode.Choices.Count()-1);
        }
    }
    
}