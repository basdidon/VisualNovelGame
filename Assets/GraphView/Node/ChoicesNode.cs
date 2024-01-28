using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Linq;
using UnityEditor.UIElements;
using UnityEditor;
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
            RemoveChild(choice.Child);
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
                SerializedObject SO = new(choicesNode);
                mainContainer.Bind(SO);

                var inputFlowPort = GetInputFlowPort();
                inputFlowPort.viewDataKey = choicesNode.InputFlowPortGuid;
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
                    DrawChoicePort(choicesNode, choicesNode.Choices.ElementAt(i),i);
                }

                // start with expanded state
                RefreshExpandedState();
            }
        }

        void DrawChoicePort(ChoicesNode choicesNode,ChoicesNode.Choice choice,int choiceIdx)
        {
            VisualElement ChoiceContainer = new();
            ChoiceContainer.style.marginTop = new StyleLength(8);
            ChoiceContainer.style.backgroundColor = Color.cyan;

            VisualElement PortsContainer = new();
            PortsContainer.style.flexDirection = FlexDirection.Row;
            PortsContainer.style.justifyContent = Justify.SpaceBetween;
            ChoiceContainer.Add(PortsContainer);

            Port isEnablePort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            isEnablePort.viewDataKey = choice.InputPortGuid;
            isEnablePort.portName = "IsEnable";
            
            var isEnablePortLabel =  isEnablePort.Q<Label>();
            isEnablePortLabel.style.color = Color.black;
            
            PortsContainer.Add(isEnablePort);

            var isEnableToggle = new Toggle() { bindingPath = $"choices.Array.data[{choiceIdx}].<IsEnable>k__BackingField" };
            isEnablePort.Add(isEnableToggle);

            // choice output flow port
            Port choicePort = GetOutputFlowPort();
            choicePort.portName = string.Empty;
            choicePort.viewDataKey = choice.OutputPortGuid;
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
            ChoicesNode.Choice choice = new();
            choicesNode.AddChoice(choice);

            DrawChoicePort(choicesNode, choice, choicesNode.Choices.Count()-1);
        }
    }
    
}