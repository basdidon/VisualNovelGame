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
    public class Choice : BaseListElement
    {
        [Input(nameof(isEnable))]
        public bool IsEnable => GetInputValue(nameof(IsEnable),isEnable);
        public bool isEnable = true;

        [Output]
        public ExecutionFlow Output { get; }

        [Output]
        public bool True => true;

        [TextArea]
        [NodeField]
        public string name = "new choice";

        public ChoiceRecord GetRecord(GraphTree dialogueTree)
        {
            return new ChoiceRecord(IsEnable, name);
        }
    }

    [CreateNodeMenu(menuName = nameof(ChoicesNode))]
    public class ChoicesNode :BaseNode,IExecutableNode
    {
        [Input]
        public ExecutionFlow Input { get; }

        [NodeField]
        public CharacterData speaker;

        [TextArea, NodeField]
        public string questionText;

        [SerializeField]
        ListElements<Choice> choices;

        public void CreateChoice()
        {
            Debug.Log("CreateChoice");
            Debug.Log(GraphTree == null);
            var c = new Choice();
            c.Initialize(this);
            choices.Add(c);
        }

        public void RemoveChoice(Choice choice) => choices.Remove(choice);
        public void RemoveChoiceAt(int index) => choices.RemoveAt(index);       

        public override void Initialize(Vector2 position, GraphTree dialogueTree)
        {
            base.Initialize(position, dialogueTree);

            //choices = new();
            choices = new(this);
            CreateChoice();
             
            SaveChanges();
        }

        // Execute Logic
        public void OnEnter()
        {
            Debug.Log("choices node executing");
            GraphTreeContorller.Instance.FireEvent(
                new ChoicesRecord(
                    new(
                        speaker.Name, 
                        questionText),
                    choices.Select(c => c.GetRecord(GraphTree)).ToArray()
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
            if (idx < 0 || idx >= choices.Count)
                throw new ArgumentOutOfRangeException();

            var selectedOutputPort = choices.ElementAt(idx).GetPortData("Output");
            GraphTreeContorller.Instance.ToNextExecutableNode(selectedOutputPort, GraphTree);
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