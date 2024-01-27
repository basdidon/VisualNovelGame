using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Linq;
using UnityEditor.UIElements;
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

    public partial class ChoicesNode : GVNodeData
    {
        [field: SerializeField] public CharacterData CharacterData { get; set; }
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
            [field: SerializeField] public bool IsEnable { get; set; }
            [field: SerializeField] public string Name { get; set; }
            [field: SerializeField] public string InputPortGuid { get; set; }
            [field: SerializeField] public string OutputPortGuid { get; set; }
            [field: SerializeField] public GVNodeData Child { get; set; }

            public Choice()
            {
                IsEnable = true;
                Name = "new choice";
                InputPortGuid = $"{Guid.NewGuid()}";
                OutputPortGuid = $"{Guid.NewGuid()}";
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

        [field: SerializeField] List<GVNodeData> Children { get; set; }

        [field: SerializeField] public string InputFlowPortGuid { get; private set; }
        public override string[] InputPortGuids => choices.Select(choice=> choice.InputPortGuid).Append(InputFlowPortGuid).ToArray();
        public override string[] OutputPortGuids => choices.Select(choice => choice.OutputPortGuid).ToArray();

        public override void Initialize(Vector2 position, DialogueTree dialogueTree)
        {
            Debug.Log("Init");
            base.Initialize(position, dialogueTree);
            InputFlowPortGuid = Guid.NewGuid().ToString();
            Children = new List<GVNodeData>();
            choices = new();
        }

        public override void AddChild(GVNodeData child)
        {
            Children.Add(child);
        }

        public override void RemoveChild(GVNodeData child)
        {
            Children.Remove(child);
        }

        public override IEnumerable<GVNodeData> GetChildren()
        {
            return Children;
        }

        public void Disconnect(string portGuid)
        {
            var portIdx = choices.FindIndex(choice => choice.OutputPortGuid == portGuid);
            if (portIdx >= 0)
            {
                choices[portIdx].Child = null;
            }
        }

        public void Connect(string portGuid, GVNodeData child)
        {
            var portIdx = choices.FindIndex(choice => choice.OutputPortGuid == portGuid);
            if (portIdx >= 0)
            {
                choices[portIdx].Child = child;
            }
        }
    }

    [CustomGraphViewNode(typeof(ChoicesNode))]
    public class CustomChoicesGraphViewNode : GraphViewNode
    {
        public override void OnDrawNodeView(GVNodeData nodeData)
        {
            if (nodeData is ChoicesNode choicesNode)
            {
                var inputFlowPort = GetInputFlowPort();
                inputFlowPort.viewDataKey = choicesNode.InputFlowPortGuid;
                inputContainer.Add(inputFlowPort);

                Button addCondition = new() { text = "Add Choice" };
                addCondition.clicked += () => OnAddChoice(choicesNode);
                mainContainer.Insert(1, addCondition);

                choicesNode.QuestionText ??= string.Empty;

                // CharacterData ObjectField
                ObjectField characterDataObjectField = new()
                {
                    objectType = typeof(CharacterData),
                    value = choicesNode.CharacterData
                };
                characterDataObjectField.RegisterValueChangedCallback(e => choicesNode.CharacterData = (CharacterData)e.newValue);
                mainContainer.Insert(1, characterDataObjectField);

                // Custom extension
                VisualElement customVisualElement = new();

                var textArea = new TextField()
                {
                    value = choicesNode.QuestionText,
                    multiline = true,
                };
                textArea.RegisterValueChangedCallback((e) => choicesNode.QuestionText = e.newValue);
                customVisualElement.Add(textArea);

                extensionContainer.Add(customVisualElement);

                // output port
                foreach (var choice in choicesNode.Choices)
                {
                    DrawChoicePort(choicesNode, choice);
                }

                // start with expanded state
                RefreshExpandedState();
            }
        }

        void DrawChoicePort(ChoicesNode choicesNode,ChoicesNode.Choice choice)
        {
            VisualElement ChoiceContainer = new();
            ChoiceContainer.style.marginTop = new StyleLength(8);
            ChoiceContainer.style.backgroundColor = Color.cyan;

            VisualElement PortsContainer = new();
            PortsContainer.style.flexDirection = FlexDirection.Row;
            PortsContainer.style.justifyContent = Justify.SpaceBetween;
            ChoiceContainer.Add(PortsContainer);

            Port isEnablePort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            isEnablePort.portName = "IsEnable";
            
            var isEnablePortLabel =  isEnablePort.Q<Label>();
            isEnablePortLabel.style.color = Color.black;
            
            PortsContainer.Add(isEnablePort);

            List<string> booleanStrings = new() { "true", "false" };
            var isEnableDropdown = new DropdownField(booleanStrings,choice.IsEnable ? booleanStrings[0] : booleanStrings[1]);

            isEnableDropdown.RegisterValueChangedCallback(e => choice.IsEnable = e.newValue == booleanStrings[0]);
            isEnablePort.Add(isEnableDropdown);

            Port choicePort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(ExecutionFlow));
            choicePort.portName = string.Empty;
            choicePort.portColor = Color.yellow;
            choicePort.viewDataKey = choice.OutputPortGuid;
            PortsContainer.Add(choicePort);
            
            TextField choiceTxtField = new() { value = choice.Name };
            ChoiceContainer.Add(choiceTxtField);
            
            
            Button deleteChoiceBtn = new() { text = "X" };
            ChoiceContainer.Add(deleteChoiceBtn);

            extensionContainer.Add(ChoiceContainer);

            choiceTxtField.RegisterCallback<InputEvent>((ev) =>
            {
                TextField txtField = ev.currentTarget as TextField;
                choice.Name = ev.newData;
            });

            deleteChoiceBtn.clicked += () =>
            {
                extensionContainer.Remove(ChoiceContainer);
                choicesNode.RemoveChoice(choice);
            };
        }

        void OnAddChoice(ChoicesNode choicesNode)
        {
            ChoicesNode.Choice choice = new();
            choicesNode.AddChoice(choice);

            DrawChoicePort(choicesNode, choice);
        }
    }
    
}