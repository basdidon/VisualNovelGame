using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace BasDidon.Dialogue.NodeTemplate
{
    using VisualGraphView;

    public record ChoicesRecord : ICustomEvent
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
            IsEnableInputPortData = new(Direction.Input, "isEnable");
            OutputFlowPortData = new(Direction.Output,"OutputFlow");
        }

        public ChoiceRecord GetRecord(DialogueTree dialogueTree)
        {
            bool isEnable = dialogueTree.GetInputValue(IsEnableInputPortData.PortGuid, IsEnable);

            return new ChoiceRecord(isEnable, Name);
        }
    }

    [CreateNodeMenu(menuName = "ChoicesNode")]
    public class ChoicesNode :BaseNode,IExecutableNode
    {
        [Input]
        public ExecutionFlow Input { get; }

        [NodeField]
        public CharacterData speaker;
        public CharacterData Speaker
        {
            get
            {
                if (speaker == null)
                    throw new System.NullReferenceException();
                return speaker;
            }
        }

        [TextArea, NodeField]
        public string questionText;

        ListElement<Choice> new_choices;

        [SerializeField]
        [ListField(typeof(ChoicesGraphListView))]
        List<Choice> choices;
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

        public void RemoveChoiceAt(int index)
        {
            choices.RemoveAt(index);
        }

        public override void Initialize(Vector2 position, DialogueTree dialogueTree)
        {
            base.Initialize(position, dialogueTree);

            choices = new();

            SaveChanges();
        }

        // Execute Logic
        public void OnEnter()
        {
            Debug.Log("choices node executing");
            GraphTreeContorller.Instance.FireEvent(
                new ChoicesRecord(
                    new(
                        Speaker.Name, 
                        questionText),
                    Choices.Select(c => c.GetRecord(DialogueTree)).ToArray()
                )
            );
        }

        public void OnExit(){}

        public void Action(IBaseAction action)
        {
            if(action is SelectChoiceAction select)
            {
                SelectChoice(select.ChoiceIndex);
            }
        }

        void SelectChoice(int idx)
        {
            if (idx < 0 || idx >= Choices.Count)
                throw new ArgumentOutOfRangeException();

            var selectedOutputPort = Choices.ElementAt(idx).OutputFlowPortData;
            GraphTreeContorller.Instance.ToNextExecutableNode(selectedOutputPort, DialogueTree);
        }
    }

    public class SelectChoiceAction:IBaseAction
    {
        public int ChoiceIndex { get; }

        public SelectChoiceAction(int choiceAction)
        {
            ChoiceIndex = choiceAction;
        }
    }
}