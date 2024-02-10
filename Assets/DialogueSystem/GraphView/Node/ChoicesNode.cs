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
        public ChoicesNode.Choice Choices { get; }
        public string[] ChoicesText { get; }
        public bool[] ChoicesEnable { get; }

        public ChoicesRecord(DialogueRecord dialogueRecord, string[] choicesText, bool[] choicesEnable)
        {
            DialogueRecord = dialogueRecord;
            ChoicesText = choicesText;
            ChoicesEnable = choicesEnable;
        }
        /*
        public ChoicesRecord(CharacterData characterData, string dialogueText, string[] choicesText, bool[] choicesEnable) 
            : this(new(characterData, dialogueText), choicesText, choicesEnable)
        { }*/
    }

    public class ChoicesNode : BaseNode, IExecutableNode
    {
        [field: SerializeField] public string SpeakerName { get; private set; }
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

        public void OnEnter()
        {
            Debug.Log("choices node executing");
            DialogueManager.Instance.OnSelectChoicesEvent(new ChoicesRecord(
                new(SpeakerName, QuestionText),
                Choices.Select(c => c.Name).ToArray(),
                Choices.Select(c => {
                    var _isEnable = DialogueTree.GetData(c.IsEnableInputPortData.PortGuid);
                    return _isEnable != null ? (bool)_isEnable : c.IsEnable;
                }).ToArray()
            ));
        }

        public void OnExit(){}

        public void SelectChoice(int idx)
        {
            if (idx < 0 || idx >= Choices.Count)
            {
                throw new ArgumentOutOfRangeException();
            }
            else
            {
                DialogueManager.Instance.CurrentNode = Choices.ElementAt(idx).OutputFlowPortData.GetConnectedNodeOfType<IExecutableNode>().FirstOrDefault();
            }
        }
    }
}