using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEditor;

namespace H8.FlowGraph.NodeTemplate
{
    public class SelectChoiceAction : IBaseAction
    {
        public int ChoiceIndex { get; }

        public SelectChoiceAction(int choiceAction)
        {
            ChoiceIndex = choiceAction;
        }
    }
    public record ChoicesEvent : ICustomEvent
    {
        public DialogueEvent DialogueRecord { get; }
        public ChoiceRecord[] ChoiceRecords { get; }

        public ChoicesEvent(DialogueEvent dialogueRecord, ChoiceRecord[] choiceRecords)
        {
            DialogueRecord = dialogueRecord;
            ChoiceRecords = choiceRecords;
        }
    }

    public record ChoiceRecord
    {
        public bool IsEnable { get; }
        public string ChoiceText { get; }

        public ChoiceRecord(bool isEnable, string choiceText)
        {
            IsEnable = isEnable;
            ChoiceText = choiceText;
        }
    }

    [Serializable]
    public class Choice : BaseListElement
    {
        [Input(nameof(isEnable))]
        public bool IsEnable => GetInputValue(nameof(IsEnable), isEnable);
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
            Debug.Log("GetRecord");
            return new ChoiceRecord(IsEnable, name);
        }
    }

    [CreateNodeMenu("Dialogue/Choice")]
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

            choices = new(this);
            CreateChoice();
             
            SaveChanges();
        }

        // Execute Logic
        public void OnEnter(GraphTreeController controller)
        {
            Debug.Log("choices node executing");
            controller.FireEvent(
                new ChoicesEvent(
                    new(
                        speaker?.Name, 
                        questionText),
                    choices.Select(c => c.GetRecord(GraphTree)).ToArray()
                )
            );
        }

        public void OnExit(GraphTreeController controller) { }

        public void Action(GraphTreeController controller, IBaseAction action)
        {
            if(action is SelectChoiceAction select)
            {
                var idx = select.ChoiceIndex;

                if (idx < 0 || idx >= choices.Count)
                    throw new ArgumentOutOfRangeException();

                var selectedOutputPort = choices.ElementAt(idx).GetPortData("Output");
                controller.ToNextExecutableNode(selectedOutputPort, GraphTree);
            }
        }
    }


}