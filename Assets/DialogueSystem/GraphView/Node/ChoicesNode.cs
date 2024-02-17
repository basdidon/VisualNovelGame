using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace BasDidon.Dialogue.VisualGraphView
{
    public record ChoicesRecord
    {
        public DialogueRecord DialogueRecord { get; }
        public ChoiceRecord[] ChoiceRecords { get; }

        public ChoicesRecord(DialogueRecord dialogueRecord, ChoiceRecord[] choiceRecords)
        {
            DialogueRecord = dialogueRecord;
            ChoiceRecords = choiceRecords;
        }
    }

    public record ChoiceRecord
    {
        public bool IsEnable { get; }
        public string ChoiceText { get; }

        public ChoiceRecord(bool isEnable,string choiceText)
        {
            IsEnable = isEnable;
            ChoiceText = choiceText;
        }
    }

    [Serializable]
    public class Choice
    {
        [field: SerializeField] public bool IsEnable { get; private set; }
        [field: SerializeField] public string Name { get; private set; }

        [field: SerializeField] public PortData IsEnableInputPortData { get; private set; }
        [field: SerializeField] public PortData OutputFlowPortData { get; private set; }

        public Choice()
        {
            IsEnable = true;
            Name = "new choice";
            IsEnableInputPortData = new(Direction.Input,typeof(bool));
            OutputFlowPortData = new(Direction.Output,typeof(ExecutionFlow));
        }

        public ChoiceRecord GetRecord(DialogueTree dialogueTree)
        {
            var _isEnable = dialogueTree.GetValueByPort(IsEnableInputPortData.PortGuid);
            var isEnable = _isEnable != null ? (bool)_isEnable : IsEnable;

            return new ChoiceRecord(isEnable, Name);
        }
    }

    public class ChoicesNode : ExecutableNode
    {
        [NodeField]
        public string speakerName;

        [TextArea, NodeField]
        public string questionText;

        [SerializeField] List<Choice> choices;
        public IReadOnlyList<Choice> Choices => choices;

        public void CreateChoice()
        {
            Debug.Log("CreateChoice");
            choices.Add(new());
        }

        public void RemoveChoice(Choice choice)
        {
            choices.Remove(choice);
        }

        public override void Initialize(Vector2 position, DialogueTree dialogueTree)
        {
            base.Initialize(position, dialogueTree);

            choices = new();

            SaveChanges();
        }

        // Execute Logic
        public override void OnEnter()
        {
            Debug.Log("choices node executing");
            DialogueManager.Instance.OnSelectChoicesEvent(new ChoicesRecord(
                new(speakerName, questionText),
                Choices.Select(c => c.GetRecord(DialogueTree)).ToArray()
            ));
        }

        public override void OnExit(){}

        public void SelectChoice(int idx)
        {
            if (idx < 0 || idx >= Choices.Count)
                throw new ArgumentOutOfRangeException();

            var selectedOutputPort = Choices.ElementAt(idx).OutputFlowPortData;
            var selectedNode = DialogueTree.GetConnectedNodes<IExecutableNode>(selectedOutputPort).FirstOrDefault();
            DialogueManager.Instance.CurrentNode = selectedNode;
        }
    }
}