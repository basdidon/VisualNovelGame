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
        [Input]
        public ExecutionFlow Input { get; }
        [Output]
        public ExecutionFlow Output { get; }


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
            DialogueManager.Instance.SetNextNode(GetPortData(nameof(Output)), DialogueTree);
        }
    }

}