using UnityEngine;
using System.Linq;

namespace BasDidon.Dialogue.VisualGraphView
{
    public record DialogueRecord
    {
        public string SpeakerName { get; }
        public string DialogueText { get; }

        public DialogueRecord(string speakerName,string dialogueText)
        {
            SpeakerName = speakerName;
            DialogueText = dialogueText ?? string.Empty;
        }
    }

    public class DialogueNode : BaseNode, IExecutableNode
    {
        [Port(PortDirection.Input)]
        public ExecutionFlow input;

        [Port(PortDirection.Output)]
        public ExecutionFlow output;

        [field: Selector]
        public Characters speaker;
            
        [TextArea, NodeField]
        public string DialogueText  = string.Empty;

        public void OnEnter()
        {
            Debug.Log("dialogue node executing");
            DialogueManager.Instance.OnNewDialogueEventInvoke(new(speaker.ToString(), StringHelper.GetValueFromSyntax(DialogueText)));
        }

        public void OnExit(){}

        public void Next()
        {
            DialogueManager.Instance.CurrentNode = DialogueTree.GetConnectedNodes<IExecutableNode>(GetPortData("output")).FirstOrDefault();
        }
    }

}