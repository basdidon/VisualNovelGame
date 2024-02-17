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

    public class DialogueNode : ExecutableNode
    {
        [NodeField]
        public string SpeakerName;

        [TextArea, NodeField]
        public string DialogueText  = string.Empty;

        public override void OnEnter()
        {
            Debug.Log("dialogue node executing");
            DialogueManager.Instance.OnNewDialogueEventInvoke(new(SpeakerName, StringHelper.GetValueFromSyntax(DialogueText)));
        }

        public override void OnExit(){}

        public void Next()
        {
            DialogueManager.Instance.CurrentNode = DialogueTree.GetConnectedNodes<IExecutableNode>(GetPortData("Output")).FirstOrDefault();
        }
    }

}