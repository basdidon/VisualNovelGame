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
        public string[] ChoicesText { get; }

        public ChoicesRecord(DialogueRecord dialogueRecord, string[] choicesText)
        {
            DialogueRecord = dialogueRecord;
            ChoicesText = choicesText;
        }

        public ChoicesRecord(CharacterData characterData, string dialogueText, string[] choicesText)
        {
            DialogueRecord = new(characterData,dialogueText);
            ChoicesText = choicesText;
        }
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
            [field: SerializeField] public string Name { get; set; }
            [field: SerializeField] public string OutputPortGuid { get; set; }
            [field: SerializeField] public GVNodeData Child { get; set; }

            public Choice()
            {
                Name = "new choice";
                OutputPortGuid = $"{Guid.NewGuid()}";
            }
        }

        public override void Execute()
        {
            DialogueManager.Instance.OnSelectChoicesEvent(new(CharacterData, QuestionText, choices.Select(c => c.Name).ToArray()));
        }

        [field: SerializeField] List<GVNodeData> Children { get; set; }

        public override string[] OutputPortGuids => choices.Select(choice => choice.OutputPortGuid).ToArray();

        public override void Initialize(Vector2 position, DialogueTree dialogueTree)
        {
            base.Initialize(position, dialogueTree);
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

                DrawInputPort();

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
            Port choicePort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            choicePort.portName = string.Empty;
            choicePort.viewDataKey = choice.OutputPortGuid;
            Button deleteChoiceBtn = new() { text = "X" };
            TextField choiceTxtField = new() { value = choice.Name };
            choiceTxtField.style.width = 100;
            choicePort.Add(choiceTxtField);
            choicePort.Add(deleteChoiceBtn);

            outputContainer.Add(choicePort);

            choiceTxtField.RegisterCallback<InputEvent>((ev) =>
            {
                TextField txtField = ev.currentTarget as TextField;
                choice.Name = ev.newData;
            });

            deleteChoiceBtn.clicked += () =>
            {
                outputContainer.Remove(choicePort);
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