using UnityEngine;

namespace BasDidon.Dialogue.NodeTemplate
{
    using VisualGraphView;

    public record DialogueRecord : ICustomEvent
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
        public string DialogueText  = string.Empty;

        public void OnEnter()
        {
            Debug.Log("dialogue node executing");
            DialogueManager.Instance.FireEvent(new DialogueRecord(Speaker.Name, StringHelper.GetValueFromSyntax(DialogueText)));
        }

        public void OnExit(){}

        public void Action(IBaseAction action)
        {
            if(action is NextDialogueAction)
            {
                DialogueManager.Instance.ToNextExecutableNode(GetPortData(nameof(Output)), DialogueTree);
            }
        }
    }


    public class NextDialogueAction : IBaseAction{}

}